using Firebase;
using Firebase.Firestore;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreManager : MonoBehaviour
{
    FirebaseFirestore db;

    async void Start()
    {
        // Verificar dependencias Firebase
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status == DependencyStatus.Available)
        {
            Debug.Log("Firebase Ready");

            // Obtener instancia Firestore
            db = FirebaseFirestore.DefaultInstance;

            // Enviar datos
            await SendData();
        }
        else
        {
            Debug.LogError("Firebase error: " + status);
        }
    }

    async Task SendData()
    {
        // Referencia:
        // users/player1
        DocumentReference docRef = db.Collection("users").Document("player1");

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "username", "Angel" },
            { "score", 1500 },
            { "level", 3 },
            { "online", true }
        };

        await docRef.SetAsync(data);

        Debug.Log("Datos enviados a Firestore");
    }
}