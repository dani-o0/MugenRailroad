﻿using UnityEngine;
using UnityEngine.Serialization;
using NeoSaveGames.Serialization;
using NeoSaveGames;
using System.Collections;

namespace NeoFPS.ModularFirearms
{
	[HelpURL("https://docs.neofps.com/manual/weaponsref-mb-spreadhitscanshooter.html")]
	public class SpreadHitscanShooter : BaseShooterBehaviour, IUseCameraAim
    {
        [Header("Shooter Settings")]

        [SerializeField, Tooltip("The maximum distance that the weapon will register a hit.")]
		private float m_MaxDistance = 1000f;

        [SerializeField, NeoObjectInHierarchyField(true, required = true), Tooltip("The transform that the bullet actually fires from")]
		private Transform m_MuzzleTip = null;

        [SerializeField, Tooltip("The layers bullets will collide with.")]
        private LayerMask m_Layers = PhysicsFilter.Masks.BulletBlockers;

        [SerializeField, Tooltip("Should the shot be tested against trigger colliders.")]
        private bool m_QueryTriggerColliders = false;

        [SerializeField, Tooltip("The minimum angle from forward (in degrees) of the shot (at full accuracy)")]
        private float m_MinAimOffset = 0f;

        [SerializeField, Tooltip("The maximum angle from forward (in degrees) of the shot (at zero accuracy)")]
        private float m_MaxAimOffset = 10f;

        [FormerlySerializedAs("m_UseCameraForward")] // Remove this
        [SerializeField, Tooltip("When set to use camera aim, the gun first casts from the FirstPersonCamera's aim transform, and then from the muzzle tip to that point to get more accurate firing.")]
        private UseCameraAim m_UseCameraAim = UseCameraAim.HipFireOnly;

        [SerializeField, Tooltip("How many pellets are fired each shot")]
		private int m_BulletCount = 8;

        [SerializeField, Tooltip("The spread of the cone in degrees")]
		private float m_Cone = 15f;

		[Header ("Tracer")]

        [SerializeField, NeoPrefabField(typeof(IPooledHitscanTrail)), Tooltip("The optional pooled tracer prototype to use (must implement the IPooledHitscanTrail interface)")]
        private PooledObject m_TracerPrototype = null;

        [SerializeField, Tooltip("How size (thickness/radius) of the tracer line")]
        private float m_TracerSize = 0.01f;

        [SerializeField, Tooltip("How long the tracer line will last")]
		private float m_TracerDuration = 0.05f;

        [SerializeField, Tooltip("How many pellets are required per tracer line")]
		private int m_ShotsPerTracer = 2;
        
        private ShotInfo[] m_Shots = null;
        private WaitForEndOfFrame m_WaitForEndOfFrame = new WaitForEndOfFrame();

        struct ShotInfo
        {
            public bool didHit;
            public RaycastHit hit;
            public Vector3 shotDirection;
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
			if (m_MaxDistance < 0.5f)
				m_MaxDistance = 0.5f;
			if (m_BulletCount < 2)
                m_BulletCount = 2;
            if (m_ShotsPerTracer < 0)
                m_ShotsPerTracer = 0;
            if (m_TracerDuration < 0f)
                m_TracerDuration = 0f;
            m_Cone = Mathf.Clamp(m_Cone, 0.1f, 90f);
            m_TracerSize = Mathf.Clamp(m_TracerSize, 0.001f, 0.25f);
            m_MinAimOffset = Mathf.Clamp(m_MinAimOffset, 0f, 45f);
            m_MaxAimOffset = Mathf.Clamp(m_MaxAimOffset, 0f, 45f);
        }
        #endif

        public LayerMask collisionLayers
        {
            get { return m_Layers; }
            set { m_Layers = value; }
        }

        public override bool isModuleValid
        {
            get { return m_MuzzleTip != null && m_Layers != 0; }
        }

        public UseCameraAim useCameraAim
        {
            get { return m_UseCameraAim; }
            set { m_UseCameraAim = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            m_Shots = new ShotInfo[m_BulletCount];
        }

        protected virtual float GetModifiedAimOffset(float unmodified)
        {
            return unmodified;
        }

        public override void Shoot (float accuracy, IAmmoEffect effect)
		{
			// Just return if there is no effect
			if (effect == null)
				return;

            // Get root game object to prevent impacts with body
            Transform ignoreRoot = null;
            if (firearm.wielder != null)
                ignoreRoot = firearm.wielder.gameObject.transform;

            // Get the forward vector
            Vector3 m_MuzzlePosition = m_MuzzleTip.position;
            Vector3 startPosition = m_MuzzlePosition;
            Vector3 forwardVector = m_MuzzleTip.forward;

            bool useCamera = false;
            if (firearm.wielder != null)
            {
                switch (m_UseCameraAim)
                {
                    case UseCameraAim.HipAndAimDownSights:
                        useCamera = true;
                        break;
                    case UseCameraAim.AimDownSightsOnly:
                        if (firearm.aimer != null)
                            useCamera = firearm.aimer.isAiming;
                        break;
                    case UseCameraAim.HipFireOnly:
                        if (firearm.aimer != null)
                            useCamera = !firearm.aimer.isAiming;
                        else
                            useCamera = true;
                        break;
                }
            }
            if (useCamera)
            {
                Transform aimTransform = firearm.wielder.fpCamera.aimTransform;
                startPosition = aimTransform.position;
                forwardVector = aimTransform.forward;
            }

            // Get the direction (with accuracy offset)
            float spread = GetModifiedAimOffset(Mathf.Lerp(m_MinAimOffset, m_MaxAimOffset, 1f - accuracy));
            if (spread > 0.0001f)
            {
                Quaternion randomRot = UnityEngine.Random.rotationUniform;
                forwardVector = Quaternion.Slerp(Quaternion.identity, randomRot, spread / 360f) * forwardVector;
            }

            // Get the damage source
            IDamageSource damageSource = firearm as IDamageSource;

            // Fire individual shots
            var queryTriggers = m_QueryTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
            for (int i = 0; i < m_BulletCount; ++i)
			{
				// Get the direction
				Vector3 randomDir = UnityEngine.Random.onUnitSphere;
                m_Shots[i].shotDirection = Vector3.Slerp (forwardVector, randomDir, m_Cone / 360f);

				// Check for raycast hit
				Ray ray = new Ray (startPosition, m_Shots[i].shotDirection);
                Vector3 targetPoint = startPosition + (m_Shots[i].shotDirection * m_MaxDistance);
                bool didHit = PhysicsExtensions.RaycastNonAllocSingle(ray, out m_Shots[i].hit, m_MaxDistance, m_Layers, ignoreRoot, queryTriggers);
                if (!didHit)
                    m_Shots[i].hit.point = targetPoint;

                // Double check hit from gun muzzle to prevent near scenery weirdness
                if (useCamera)
                {
                    m_Shots[i].shotDirection = m_Shots[i].hit.point - m_MuzzlePosition;
                    m_Shots[i].shotDirection.Normalize();
                    ray = new Ray(m_MuzzlePosition, m_Shots[i].shotDirection);
                    didHit = PhysicsExtensions.RaycastNonAllocSingle(ray, out m_Shots[i].hit, m_MaxDistance, m_Layers, ignoreRoot, queryTriggers);
                    if (!didHit)
                        m_Shots[i].hit.point = targetPoint;
                }

                // Record if did hit
                m_Shots[i].didHit = didHit;
            }

            // Show the tracers
            if (m_TracerPrototype != null)
                StartCoroutine(ShowTracers());

            // Process hits
            for (int i = 0; i < m_Shots.Length; ++i)
            {
                if (m_Shots[i].didHit)
                    effect.Hit(m_Shots[i].hit, m_Shots[i].shotDirection, m_Shots[i].hit.distance, float.PositiveInfinity, damageSource);
            }

            base.Shoot (accuracy, effect);
        }

        IEnumerator ShowTracers()
        {
            yield return m_WaitForEndOfFrame;
            Vector3 muzzlePosition = m_MuzzleTip.position;
            for (int i = 0; i < m_Shots.Length; ++i)
            {
                if (i % m_ShotsPerTracer == 0)
                {
                    var tracer = PoolManager.GetPooledObject<IPooledHitscanTrail>(m_TracerPrototype);
                    tracer.Show(muzzlePosition, m_Shots[i].hit.point, m_TracerSize, m_TracerDuration);
                }
            }
        }

        private static readonly NeoSerializationKey k_LayersKey = new NeoSerializationKey("layers");

        public override void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            base.WriteProperties(writer, nsgo, saveMode);
            writer.WriteValue(k_LayersKey, m_Layers);
        }

        public override void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            base.ReadProperties(reader, nsgo);
            int layers = m_Layers;
            if (reader.TryReadValue(k_LayersKey, out layers, layers))
                collisionLayers = layers;

        }
    }
}