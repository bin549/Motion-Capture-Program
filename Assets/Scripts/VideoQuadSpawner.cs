using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideoQuadSpawner : MonoBehaviour
{
    public VideoQuad videoQuad;
    public List<VideoQuad> videoQuads;
    private int currentVideoQuadIndex = -1;
    public Transform spawnPoint;
    public int xColumn, yRow;
    private Animator animator;
    [SerializeField] private AppSettings appSettings;
    string[] videos;

    private void Awake()
    {
        appSettings = GameObject.FindObjectOfType<AppSettings>();
        videoQuads = new List<VideoQuad>();
        animator = GetComponent<Animator>();
        videos = appSettings.GetVideos();
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                SpawnVideoQuad(x, y);
            }
        }
        for (int i = 0; i < videos.Length; i++)
        {
            videoQuads[i].SetupVideo(videos[i]); 
        }
    }
    
    private void Update()
    {
        VideoSelected();
    }

    public void SpawnVideoQuad(int x, int y)
    {
        VideoQuad newVideoQuad = Instantiate(videoQuad, spawnPoint.position + new Vector3(80 * y, -66 * x, 0), Quaternion.identity) as VideoQuad;
        newVideoQuad.transform.SetParent(transform);
        videoQuads.Add(newVideoQuad);
        newVideoQuad.gameObject.GetComponent<Renderer>().enabled = false;
    }

    private void VideoSelected()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentVideoQuadIndex != -1)
            {
                videoQuad.UnSelected();
                currentVideoQuadIndex--;
            }
            if (currentVideoQuadIndex == -1)
            {
                currentVideoQuadIndex = videos.Length - 1;
            }
            videoQuad = videoQuads[currentVideoQuadIndex];
            videoQuad.Selected();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentVideoQuadIndex != -1)
                videoQuad.UnSelected();
            currentVideoQuadIndex++;
            currentVideoQuadIndex %= videos.Length;
            videoQuad = videoQuads[currentVideoQuadIndex];
            videoQuad.Selected();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (videoQuad != null)
            {
                videoQuad.Confirm();
            }
        }
    }
}
