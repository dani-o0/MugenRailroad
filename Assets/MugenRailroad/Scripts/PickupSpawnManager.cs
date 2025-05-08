using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS;

/// <summary>
/// Script que gestiona la aparición de objetos comprables (PurchasablePickup) según su tipo
/// y el número de vagón actual, basándose en una tabla de probabilidades configurada.
/// También gestiona la distribución de los objetos en puntos de spawn específicos.
/// </summary>
public class PickupSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnProbability
    {   
        [Tooltip("Tipo de objeto comprable")]
        public PickupType pickupType;
        
        [Tooltip("Probabilidad de aparición en el vagón 1 (0-100%)")]
        [Range(0, 100)]
        public float wagonOneProbability = 100f;
        
        [Tooltip("Probabilidad de aparición en el vagón 2 (0-100%)")]
        [Range(0, 100)]
        public float wagonTwoProbability = 100f;
        
        [Tooltip("Probabilidad de aparición en el vagón 3 (0-100%)")]
        [Range(0, 100)]
        public float wagonThreeProbability = 100f;
        
        [Tooltip("Probabilidad de aparición en el vagón 4 (0-100%)")]
        [Range(0, 100)]
        public float wagonFourProbability = 100f;
        
        [Tooltip("Probabilidad de aparición en el vagón 5 (0-100%)")]
        [Range(0, 100)]
        public float wagonFiveProbability = 100f;
        
        [Tooltip("Probabilidad de aparición en el vagón del jefe (0-100%)")]
        [Range(0, 100)]
        public float wagonBossProbability = 100f;
    }
    
    [Header("Configuración de Probabilidades")]
    [SerializeField, Tooltip("Tabla de probabilidades de aparición por tipo de objeto y número de vagón")]
    private SpawnProbability[] m_SpawnProbabilities = new SpawnProbability[]
    {
        // Valores predeterminados según los requisitos
        new SpawnProbability { pickupType = PickupType.Base, wagonOneProbability = 100f, wagonTwoProbability = 100f, wagonThreeProbability = 100f, wagonFourProbability = 100f, wagonFiveProbability = 100f, wagonBossProbability = 100f },
        new SpawnProbability { pickupType = PickupType.Golden, wagonOneProbability = 0f, wagonTwoProbability = 20f, wagonThreeProbability = 40f, wagonFourProbability = 60f, wagonFiveProbability = 80f, wagonBossProbability = 100f },
        new SpawnProbability { pickupType = PickupType.Mugen, wagonOneProbability = 0f, wagonTwoProbability = 0f, wagonThreeProbability = 0f, wagonFourProbability = 0f, wagonFiveProbability = 0f, wagonBossProbability = 100f }
    };
    
    [Header("Configuración de Pickups")]
    [SerializeField, Tooltip("Lista de prefabs de pickups que pueden aparecer en la tienda")]
    private List<GameObject> m_PickupPrefabs = new List<GameObject>();
    
    [SerializeField, Tooltip("Lista de pickups instanciados en la escena")]
    private List<PurchasablePickup> m_InstantiatedPickups = new List<PurchasablePickup>();
    
    // Lista de pickups disponibles para procesar (reemplaza a m_AvailablePickups)
    private List<PurchasablePickup> m_AvailablePickups = new List<PurchasablePickup>();
    
    [Header("Configuración de Spawns")]
    [SerializeField, Tooltip("Lista de puntos de spawn donde pueden aparecer los pickups (máximo un pickup por punto)")]
    private List<Transform> m_SpawnPoints = new List<Transform>();
    
    private bool m_ShowDebugMessages = false;
    
    private GameManager m_GameManager;
    
    private void Awake()
    {
        // Obtener referencia al GameManager
        m_GameManager = GameManager.Instance;
        if (m_GameManager == null)
        {
            Debug.LogError("PickupSpawnManager: No se encontró el GameManager");
            return;
        }
    }
    
    private void Start()
    {
        // Esperar un frame para asegurarnos de que todos los objetos estén inicializados
        StartCoroutine(ProcessPickupsNextFrame());
    }
    
    /// <summary>
    /// Limpia los pickups instanciados previamente y los elimina de la escena
    /// </summary>
    private void CleanupInstantiatedPickups()
    {
        // Destruir todos los pickups instanciados previamente
        foreach (PurchasablePickup pickup in m_InstantiatedPickups)
        {
            if (pickup != null)
            {
                Destroy(pickup.gameObject);
            }
        }
        
        // Limpiar la lista
        m_InstantiatedPickups.Clear();
        m_AvailablePickups.Clear();
    }
    
    private IEnumerator ProcessPickupsNextFrame()
    {
        // Esperar al siguiente frame
        yield return null;
        
        // Procesar todos los PurchasablePickup en la escena
        ProcessAllPickups();
    }
    
    /// <summary>
    /// Procesa los prefabs de pickups, instancia los objetos y determina si deben aparecer
    /// según las probabilidades configuradas para el vagón actual.
    /// Distribuye los pickups entre los puntos de spawn disponibles.
    /// </summary>
    private void ProcessAllPickups()
    {
        // Limpiar pickups instanciados previamente
        CleanupInstantiatedPickups();
        
        // Obtener el número de vagón actual
        int currentWagonNumber = m_GameManager.CurrentWagonNumber;
        
        // Asegurarse de que el número de vagón sea al menos 1 para la lógica de spawn
        if (currentWagonNumber == 0)
        {
            if (m_ShowDebugMessages)
            {
                Debug.Log($"PickupSpawnManager: El número de vagón es 0, usando el vagón 1 para las probabilidades de spawn");
            }
            currentWagonNumber = 1;
        }
        
        // Verificar si hay prefabs disponibles
        if (m_PickupPrefabs == null || m_PickupPrefabs.Count == 0)
        {
            if (m_ShowDebugMessages)
            {
                Debug.LogWarning("PickupSpawnManager: No hay prefabs de pickups asignados en la lista");
            }
            return;
        }
        
        // Instanciar los prefabs y añadirlos a la lista de disponibles
        foreach (GameObject prefab in m_PickupPrefabs)
        {
            if (prefab == null) continue;
            
            // Instanciar el prefab (desactivado inicialmente)
            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instance.SetActive(false);
            
            // Obtener el componente PurchasablePickup
            PurchasablePickup pickup = instance.GetComponent<PurchasablePickup>();
            if (pickup != null)
            {
                // Añadir a las listas
                m_InstantiatedPickups.Add(pickup);
                m_AvailablePickups.Add(pickup);
            }
            else
            {
                // Si no tiene el componente, destruir la instancia
                if (m_ShowDebugMessages)
                {
                    Debug.LogWarning($"PickupSpawnManager: El prefab {prefab.name} no tiene el componente PurchasablePickup");
                }
                Destroy(instance);
            }
        }
        
        if (m_ShowDebugMessages)
        {
            Debug.Log($"PickupSpawnManager: Procesando {m_AvailablePickups.Count} objetos comprables en el vagón {currentWagonNumber}");
        }
        
        // Verificar si hay puntos de spawn disponibles
        if (m_SpawnPoints == null || m_SpawnPoints.Count == 0)
        {
            if (m_ShowDebugMessages)
            {
                Debug.LogWarning("PickupSpawnManager: No hay puntos de spawn configurados. Los pickups no aparecerán.");
            }
            
            // Desactivar todos los pickups si no hay puntos de spawn
            foreach (PurchasablePickup pickup in m_AvailablePickups)
            {
                if (pickup != null)
                    pickup.gameObject.SetActive(false);
            }
            return;
        }
        
        // Crear una lista de pickups que deberían aparecer según las probabilidades
        List<PurchasablePickup> pickupsToSpawn = new List<PurchasablePickup>();
        foreach (PurchasablePickup pickup in m_AvailablePickups)
        {
            // Verificar que el pickup no sea nulo
            if (pickup == null) continue;
            
            // Determinar si el pickup debe aparecer según su tipo y el vagón actual
            bool shouldSpawn = ShouldSpawnPickup(pickup, currentWagonNumber);
            
            if (shouldSpawn)
            {
                pickupsToSpawn.Add(pickup);
            }
            else
            {
                // Desactivar los pickups que no deben aparecer
                pickup.gameObject.SetActive(false);
            }
        }
        
        // Distribuir los pickups entre los puntos de spawn disponibles
        DistributePickupsToSpawnPoints(pickupsToSpawn);
    }
    
    /// <summary>
    /// Determina si un objeto comprable debe aparecer según su tipo y el número de vagón actual.
    /// </summary>
    /// <param name="pickup">El objeto comprable a evaluar</param>
    /// <param name="wagonNumber">El número de vagón actual</param>
    /// <returns>True si el objeto debe aparecer, False en caso contrario</returns>
    private bool ShouldSpawnPickup(PurchasablePickup pickup, int wagonNumber)
    {
        // Obtener el tipo de pickup
        PickupType pickupType = pickup.GetPickupType();
        
        // Buscar la configuración de probabilidad para este tipo
        SpawnProbability probability = GetProbabilityForType(pickupType);
        
        if (probability == null)
        {
            // Si no hay configuración, permitir que aparezca por defecto
            return true;
        }
        
        // Obtener la probabilidad según el número de vagón
        float spawnProbability = GetProbabilityForWagon(probability, wagonNumber);
        
        // Generar un número aleatorio entre 0 y 100
        float randomValue = Random.Range(0f, 100f);
        
        // El objeto aparece si el número aleatorio es menor o igual a la probabilidad
        return randomValue <= spawnProbability;
    }
    
    /// <summary>
    /// Obtiene la configuración de probabilidad para un tipo específico de objeto comprable.
    /// </summary>
    /// <param name="pickupType">El tipo de objeto comprable</param>
    /// <returns>La configuración de probabilidad o null si no se encuentra</returns>
    private SpawnProbability GetProbabilityForType(PickupType pickupType)
    {
        foreach (SpawnProbability probability in m_SpawnProbabilities)
        {
            if (probability.pickupType == pickupType)
            {
                return probability;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Obtiene la probabilidad de aparición para un vagón específico.
    /// </summary>
    /// <param name="probability">La configuración de probabilidad</param>
    /// <param name="wagonNumber">El número de vagón</param>
    /// <returns>La probabilidad de aparición (0-100)</returns>
    private float GetProbabilityForWagon(SpawnProbability probability, int wagonNumber)
    {
        // Determinar la probabilidad según el número de vagón
        switch (wagonNumber)
        {
            case 0: // Caso para cuando el número de vagón es 0 (valor por defecto)
                return probability.wagonOneProbability; // Usar la misma probabilidad que el vagón 1
            case 1:
                return probability.wagonOneProbability;
            case 2:
                return probability.wagonTwoProbability;
            case 3:
                return probability.wagonThreeProbability;
            case 4:
                return probability.wagonFourProbability;
            case 5:
                return probability.wagonFiveProbability;
            default:
                // Para el vagón del jefe u otros casos
                return probability.wagonBossProbability;
        }
    }
    
    /// <summary>
    /// Distribuye los pickups entre los puntos de spawn disponibles.
    /// Asegura que siempre haya un pickup en cada punto de spawn, reasignando
    /// pickups si es necesario para maximizar la utilización de los puntos de spawn.
    /// </summary>
    /// <param name="pickupsToSpawn">Lista de pickups que deben aparecer según las probabilidades</param>
    private void DistributePickupsToSpawnPoints(List<PurchasablePickup> pickupsToSpawn)
    {
        // Desactivar todos los pickups inicialmente
        foreach (PurchasablePickup pickup in m_AvailablePickups)
        {
            if (pickup != null)
                pickup.gameObject.SetActive(false);
        }
        
        // Crear una copia de la lista de puntos de spawn para trabajar con ella
        List<Transform> availableSpawnPoints = new List<Transform>(m_SpawnPoints);
        
        // Filtrar puntos de spawn nulos
        availableSpawnPoints.RemoveAll(sp => sp == null);
        
        // Si no hay puntos de spawn disponibles, salir
        if (availableSpawnPoints.Count == 0)
            return;
            
        // Mezclar aleatoriamente los puntos de spawn disponibles
        ShuffleList(availableSpawnPoints);
        
        // Obtener el número de vagón actual para las probabilidades
        int currentWagonNumber = m_GameManager.CurrentWagonNumber;
        if (currentWagonNumber == 0) currentWagonNumber = 1;
        
        if (m_ShowDebugMessages)
        {
            Debug.Log($"PickupSpawnManager: Asignando pickups a {availableSpawnPoints.Count} puntos de spawn");
        }
        
        // Lista para almacenar los pickups que se mostrarán finalmente
        List<PurchasablePickup> finalPickupsToShow = new List<PurchasablePickup>();
        
        // Asignar un pickup a cada punto de spawn disponible
        for (int i = 0; i < availableSpawnPoints.Count; i++)
        {
            Transform spawnPoint = availableSpawnPoints[i];
            PurchasablePickup assignedPickup = null;
            
            // Primero intentamos con los pickups que ya pasaron la verificación de probabilidad
            if (pickupsToSpawn.Count > 0 && i < pickupsToSpawn.Count)
            {
                assignedPickup = pickupsToSpawn[i];
            }
            // Si no hay suficientes pickups que pasaron la verificación, intentamos con otros
            else
            {
                // Crear una copia de la lista de pickups disponibles y mezclarla
                List<PurchasablePickup> remainingPickups = new List<PurchasablePickup>(m_AvailablePickups);
                // Eliminar los pickups que ya están asignados
                foreach (PurchasablePickup p in finalPickupsToShow)
                {
                    remainingPickups.Remove(p);
                }
                
                // Si aún quedan pickups disponibles, intentamos asignar uno
                if (remainingPickups.Count > 0)
                {
                    // Mezclar la lista para obtener un pickup aleatorio
                    ShuffleList(remainingPickups);
                    
                    // Intentar encontrar un pickup que pase la verificación de probabilidad
                    foreach (PurchasablePickup pickup in remainingPickups)
                    {
                        if (ShouldSpawnPickup(pickup, currentWagonNumber))
                        {
                            assignedPickup = pickup;
                            break;
                        }
                    }
                    
                    // Si ningún pickup pasó la verificación, simplemente tomamos el primero
                    if (assignedPickup == null && remainingPickups.Count > 0)
                    {
                        assignedPickup = remainingPickups[0];
                        
                        if (m_ShowDebugMessages)
                        {
                            Debug.Log($"PickupSpawnManager: Asignando pickup {assignedPickup.name} (Tipo: {assignedPickup.GetPickupType()}) a {spawnPoint.name} a pesar de no pasar la verificación de probabilidad");
                        }
                    }
                }
            }
            
            // Si se asignó un pickup, lo posicionamos en el punto de spawn
            if (assignedPickup != null)
            {
                finalPickupsToShow.Add(assignedPickup);
                
                // Posicionar el pickup en el punto de spawn
                assignedPickup.transform.position = spawnPoint.position;
                
                // Aplicar rotación base del punto de spawn y añadir 90 grados en Y y Z
                Quaternion additionalRotation = Quaternion.Euler(0f, 90f, 90f);
                assignedPickup.transform.rotation = spawnPoint.rotation * additionalRotation;
                
                // Activar el pickup y asegurarse de que sea visible
                assignedPickup.gameObject.SetActive(true);
                
                // Verificar si el objeto tiene componentes visuales (MeshRenderer o SkinnedMeshRenderer)
                MeshRenderer[] renderers = assignedPickup.GetComponentsInChildren<MeshRenderer>(true);
                SkinnedMeshRenderer[] skinnedRenderers = assignedPickup.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                
                if (m_ShowDebugMessages)
                {
                    Debug.Log($"PickupSpawnManager: {assignedPickup.name} (Tipo: {assignedPickup.GetPickupType()}) - Posicionado en {spawnPoint.name}");
                    Debug.Log($"PickupSpawnManager: {assignedPickup.name} tiene {renderers.Length} MeshRenderers y {skinnedRenderers.Length} SkinnedMeshRenderers");
                }
                
                // Activar todos los renderers en la jerarquía
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                
                foreach (var renderer in skinnedRenderers)
                {
                    renderer.enabled = true;
                }
            }
        }
        
        if (m_ShowDebugMessages)
        {
            Debug.Log($"PickupSpawnManager: Se han asignado {finalPickupsToShow.Count} pickups a los puntos de spawn");
        }
    }
    /// <summary>
    /// Mezcla aleatoriamente los elementos de una lista.
    /// </summary>
    /// <typeparam name="T">Tipo de elementos en la lista</typeparam>
    /// <param name="list">Lista a mezclar</param>
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}