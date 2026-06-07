using System.Collections;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public GameObject carModel;
    public Transform spawn_t;
    // Start is called before the first frase update
    void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle() {
        int cars = 5;
        for (int i = 0; i < cars; i++) {
            GameObject newCar = Instantiate(carModel);
            newCar.transform.position = spawn_t.transform.position;
            newCar.transform.rotation = spawn_t.transform.rotation;
            CarAIController controller = newCar.GetComponent<CarAIController>();
            
            if (controller == null)
            {
                controller = newCar.GetComponentInChildren<CarAIController>();
            }

            if (controller != null)
            {
                controller.CheckPointSearch = true;
                controller.isCarControlledByAI = true;
            }
            else
            {
                Debug.LogError("CarAIController component missing on the instantiated carModel prefab! Please ensure it's attached in the Inspector.", newCar);
            }
            yield return new WaitForSeconds(10);
        }
            yield return 0;
        
    }
}
