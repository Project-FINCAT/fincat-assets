using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// 플레이어가 경계(문)에 닿으면 맵이 전환되는(화면을 어둡게 하고 플레이어를 다음 방으로 옮김)
public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    CinemachineConfiner2D confiner;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPosition;
    [SerializeField] float additivePos = 2f;
    enum Direction{Up, Down, Left, Right, Teleport}
    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FadeTransition(collision.gameObject);
        }
    }

    async void FadeTransition(GameObject player)
    {

        PauseController.SetPause(true);

        Debug.Log("맵 이동 정지");

        await ScreenFader.Instance.FadeOut();

        player.GetComponent<PlayerMovement>().StopPlayer();

        Vector3 prevPos = player.transform.position;

        confiner.BoundingShape2D = mapBoundary;
        confiner.InvalidateBoundingShapeCache();

        UpdatePlayerPosition(player);

        Vector3 delta = player.transform.position - prevPos;

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        var vcam = brain.ActiveVirtualCamera as CinemachineCamera;

        if (vcam != null)
        {
            vcam.OnTargetObjectWarped(player.transform, delta);
        }

        PauseController.SetPause(false);
        
        await ScreenFader.Instance.FadeIn();
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if(direction == Direction.Teleport)
        {
            player.transform.position = teleportTargetPosition.position;

            return;
        }
        Vector3 newPos = player.transform.position;

        switch(direction)
        {
            case Direction.Up:
            newPos.y +=additivePos;
            break;
            case Direction.Down:
            newPos.y -=additivePos;
            break;
            case Direction.Left:
            newPos.x +=additivePos;
            break;
            case Direction.Right:
            newPos.x -=additivePos;
            break;
        }
        player.transform.position = newPos;
    }

}
