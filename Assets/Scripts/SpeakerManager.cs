using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerManager : MonoBehaviour
{

    [SerializeField]
    private AudioClip[] shortSongClips;

    [SerializeField]
    private GameObject speakerPrefab;

    [SerializeField]
    private int numberOfSpeakersToSpawn = 10;

    [SerializeField]
    private int radius = 1;

    private List<GameObject> spawnedSpeakers = new List<GameObject>();

    private Transform mainCameraTransform;

    private readonly WaitForSeconds timeBetweenPlayingSongs = new WaitForSeconds(5f);

    private void Start()
    {

        SetupSpeakers();

        StartCoroutine(PlaySongOnSpeaker());

    }

    private void SetupSpeakers()
    {

        for (var i = 0; i < numberOfSpeakersToSpawn; i += 1)
        {

            var angle = 360 / numberOfSpeakersToSpawn * i;

            var spawnPosition = gameObject.transform.position;

            spawnPosition.x += radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            spawnPosition.z += radius * Mathf.Cos(angle * Mathf.Deg2Rad);

            var spawnedObject = Instantiate(speakerPrefab);

            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.LookAt(mainCameraTransform);

            spawnedSpeakers.Add(spawnedObject);

        }

    }

    private IEnumerator PlaySongOnSpeaker()
    {

        var song = shortSongClips[Random.Range(0, shortSongClips.Length - 1)];
        var speaker = spawnedSpeakers[Random.Range(0, spawnedSpeakers.Count - 1)];

        var audioSource = speaker.GetComponent<AudioSource>();
        var particleSystem = speaker.GetComponentInChildren<ParticleSystem>();

        audioSource.clip = song;
        particleSystem.Play();

        audioSource.Play();

        while (audioSource.isPlaying)
        {

            yield return null;

        }

        audioSource.clip = null;
        particleSystem.Stop();
        particleSystem.Clear();

        yield return timeBetweenPlayingSongs;

        StartCoroutine(PlaySongOnSpeaker());

    }

    private void OnEnable()
    {

        mainCameraTransform = Camera.main.transform;

    }

}
