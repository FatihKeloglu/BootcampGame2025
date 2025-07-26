using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Korkusanlar : MonoBehaviour
{
    public float radius = 4f;
    public float forceStrength = 400f;
    public float forceDuration = 3f;
    public ThirdPersonCamera cameraScript; 


    [System.NonSerialized]
    internal bool FIRE = false;

    private Dictionary<Rigidbody, Vector3> activeForces = new();
    private float forceTimer = 0f;

    void Update()
    {
        Vector3 center = transform.position;
        transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);

        if (FIRE)
        {
            FIRE = false;
            ApplyForceToPlayers(center);
            cameraScript.SetBirdsEyeMode(true);
        }

        if (activeForces.Count > 0)
        {
            float deltaTime = Application.isPlaying ? Time.deltaTime : 1f / 60f;
            forceTimer += deltaTime;

            if (forceTimer < forceDuration)
            {
                foreach (var kvp in activeForces)
                {
                    if (kvp.Key != null)
                        kvp.Key.AddForce(kvp.Value * forceStrength * deltaTime, ForceMode.Force);
                }
            }
            else
            {
                activeForces.Clear();
                forceTimer = 0f;
            }
        }
    }

    void ApplyForceToPlayers(Vector3 center)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Vector3 avgPos = Vector3.zero;
        int count = 0;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(center, player.transform.position);
            if (distance <= radius)
            {
                avgPos += player.transform.position;
                count++;
            }
        }

        if (count == 0) return;

        avgPos /= count;
        activeForces.Clear();

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(center, player.transform.position);
            if (distance <= radius)
            {
                Debug.Log("Bagirarak Kacisiyor: " + player.name);
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (player.transform.position - avgPos).normalized;
                    activeForces[rb] = dir;
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Korkusanlar))]
public class KorkusanlarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Korkusanlar korkusan = (Korkusanlar)target;
        if (GUILayout.Button("FIRE"))
        {
            korkusan.FIRE = true;
        }
    }
}
#endif