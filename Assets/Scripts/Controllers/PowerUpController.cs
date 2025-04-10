using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private PowerUpSO powerUpConfig;
    [SerializeField] private Transform visual;

    private Rect bounds;
    private bool isActive = false;

    private void Awake()
    {
        PowerUpManager.Register(this);
    }

    public void Activate(Vector3 position)
    {
        transform.position = position;
        bounds = new Rect(
            position.x - powerUpConfig.width / 2f,
            position.y - powerUpConfig.height / 2f,
            powerUpConfig.width,
            powerUpConfig.height
        );
        isActive = true;
        gameObject.SetActive(true);
    }

    public void Frame()
    {
        if (!isActive) return;

        Vector3 position = transform.position;
        position.y -= powerUpConfig.fallSpeed * Time.deltaTime;
        transform.position = position;

        bounds.y = position.y - powerUpConfig.height / 2f;

        if (bounds.Overlaps(PaddlePhysics.bounds))
        {
            OnCollected();
        }
    }

    private void OnCollected()
    {
        if (powerUpConfig.type == PowerUpType.Multiball)
        {
            SpawnAdditionalBalls();
        }
        isActive = false;
        gameObject.SetActive(false);
        PowerUpManager.Unregister(this);
    }

    private void SpawnAdditionalBalls()
    {
        Vector3 paddlePos = PaddlePhysics.bounds.center;
        Vector3 ballPos = new Vector3(paddlePos.x, paddlePos.y + 3f, 0f);

        BallController ball1 = BallPool.Instance.SpawnBall(ballPos);
        BallController ball2 = BallPool.Instance.SpawnBall(ballPos);

        ball1.Direction = new Vector3(-0.5f, 1f, 0f).normalized;
        ball2.Direction = new Vector3(0.5f, 1f, 0f).normalized;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!isActive) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
#endif
} 