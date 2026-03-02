using UnityEngine;
using UnityEngine.Video; // 비디오 제어 필수
using System.IO;       // 파일 경로 제어 필수

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Header("확장자까지 정확히 적어주세요 (.mp4 등)")]
    public string videoFileName = "시작화면.mp4";

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // //memo 기기(PC, 안드로이드 등)마다 다른 StreamingAssets 절대 경로를 자동으로 찾아줌
        string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        // 찾은 경로를 Video Player에 넣고 재생!
        videoPlayer.url = videoPath;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }
}