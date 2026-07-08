using UnityEngine;
using Unity.Cinemachine;

public class WorldMapController : MonoBehaviour
{
    [Header("UI")]
    public GameObject worldMapUI;

    [Header("Player")]
    public GameObject player;

    [Header("Camera")]
    [SerializeField] private CinemachineConfiner2D confiner;

    [Header("Spawn Points")]
    public Transform homeSpawn;
    public Transform bankSpawn;
    public Transform securitiesSpawn;
    public Transform forumSpawn;
    public Transform educationSpawn;
    public Transform storeSpawn;

    [Header("Map Boundaries")]
    public PolygonCollider2D homeBoundary;
    public PolygonCollider2D bankBoundary;
    public PolygonCollider2D securitiesBoundary;
    public PolygonCollider2D forumBoundary;
    public PolygonCollider2D educationBoundary;
    public PolygonCollider2D storeBoundary;

    [Header("Unlock States")]
    public bool canEnterHome = false;
    public bool canEnterBank = false;
    public bool canEnterSecurities = false;
    public bool canEnterForum = false;
    public bool canEnterEducation = false;
    public bool canEnterStore = false;

    public void EnterHome()
    {
        if (!canEnterHome) return;

        EnterMap(homeSpawn, homeBoundary);
    }

    public void EnterBank()
    {
        if (!canEnterBank) return;

        EnterMap(bankSpawn, bankBoundary);
    }

    public void EnterSecurities()
    {
        if (!canEnterSecurities) return;

        EnterMap(securitiesSpawn, securitiesBoundary);
    }

    public void EnterForum()
    {
        if (!canEnterForum) return;

        EnterMap(forumSpawn, forumBoundary);
    }

    public void EnterEducation()
    {
        if (!canEnterEducation) return;

        EnterMap(educationSpawn, educationBoundary);
    }

    public void EnterStore()
    {
        if (!canEnterStore) return;

        EnterMap(storeSpawn, storeBoundary);
    }

    async void EnterMap(Transform spawnPoint, PolygonCollider2D boundary)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint is NULL!");
            return;
        }

        if (boundary == null)
        {
            Debug.LogError("Boundary is NULL!");
            return;
        }

        if (confiner == null)
        {
            Debug.LogError("Confiner is NULL!");
            return;
        }

        await ScreenFader.Instance.FadeOut();

        // 기존 위치 저장
        Vector3 prevPos = player.transform.position;

        // z값 유지
        Vector3 targetPos = spawnPoint.position;
        targetPos.z = player.transform.position.z;

        // 플레이어 이동
        player.transform.position = targetPos;

        // 디버그 로그
        Debug.Log("플레이어 이동 위치: " + player.transform.position);
        Debug.Log("Boundary 변경: " + boundary.name);

        // Confiner 변경
        confiner.BoundingShape2D = boundary;
        confiner.InvalidateBoundingShapeCache();

        // 이동량 계산
        Vector3 delta = player.transform.position - prevPos;

        // 카메라 Warp 처리
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        var vcam = brain.ActiveVirtualCamera as CinemachineCamera;

        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(player.transform, delta);
        }

        // 월드맵 닫기
        worldMapUI.SetActive(false);
        player.SetActive(true);

        await ScreenFader.Instance.FadeIn();
    }
}