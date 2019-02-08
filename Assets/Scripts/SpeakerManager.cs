using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpeakerManager : MonoBehaviour
{

#pragma warning disable CS0649
    [SerializeField]
    private AudioClip[] shortSongClips;

    [SerializeField]
    private GameObject speakerPrefab;
#pragma warning restore CS0649

    [SerializeField]
    private int numberOfSpeakersToSpawn = 10;

    [SerializeField]
    private int radius = 1;

    [SerializeField]
    private bool spin;

    [SerializeField]
    private float spinSpeed = 10;

    private List<GameObject> spawnedSpeakers = new List<GameObject>();

    private Transform mainCameraTransform;

    private AudioSource currentAudioSource;
    private ParticleSystem currentParticleSystem;

    private readonly WaitForSeconds timeBetweenPlayingSongs = new WaitForSeconds(2f);

    private void Start()
    {

        SetupSpeakers();

        StartCoroutine(PlaySongOnSpeaker());

    }

    private void Update()
    {

        if (spin)
        {

            gameObject.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);

        }

    }

    private void SetupSpeakers()
    {

        for (var i = 0; i < numberOfSpeakersToSpawn; i += 1)
        {

            var angle = 360 / numberOfSpeakersToSpawn * i;

            var spawnPosition = gameObject.transform.position;

            spawnPosition.x += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            spawnPosition.z += radius * Mathf.Cos(angle * Mathf.Deg2Rad);

            var spawnedObject = Instantiate(speakerPrefab, gameObject.transform);

            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.LookAt(mainCameraTransform);

            spawnedSpeakers.Add(spawnedObject);

        }

    }

    private IEnumerator PlaySongOnSpeaker()
    {

        var song = shortSongClips[Random.Range(0, shortSongClips.Length - 1)];
        var speaker = spawnedSpeakers[Random.Range(0, spawnedSpeakers.Count - 1)];

        currentAudioSource = speaker.GetComponent<AudioSource>();
        currentParticleSystem = speaker.GetComponentInChildren<ParticleSystem>();

        currentAudioSource.clip = song;
        currentParticleSystem.Play();

        currentAudioSource.Play();

        while (currentAudioSource.isPlaying)
        {

            yield return null;

        }

        StopAudioSource(currentAudioSource);
        StopParticleSystem(currentParticleSystem);

        yield return timeBetweenPlayingSongs;

        StartCoroutine(PlaySongOnSpeaker());

    }

    private static void StopAudioSource(AudioSource audioSource)
    {

        audioSource.Stop();
        audioSource.clip = null;

    }

    private static void StopParticleSystem(ParticleSystem particleSystem)
    {

        particleSystem.Stop();
        particleSystem.Clear();

    }

    public void SkipSong()
    {

        StopAudioSource(currentAudioSource);
        StopParticleSystem(currentParticleSystem);

        StopAllCoroutines();

        StartCoroutine(PlaySongOnSpeaker());

    }

    private void OnEnable()
    {

        mainCameraTransform = Camera.main.transform;

    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(SpeakerManager), true)]
public class CustomScriptableObjectEditor : Editor
{

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        var script = (SpeakerManager)target;

        if (GUILayout.Button("Skip Song"))
        {

            script.SkipSong();

        }

    }

}

#endif
