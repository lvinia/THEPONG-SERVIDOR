using UnityEngine;
using System.Globalization;

[RequireComponent(typeof(Rigidbody2D))]
public class PongBall : MonoBehaviour
{
    public float speed = 5f;                // Velocidade da bola
    private Vector2 direction;              // Direção da bola

    public Vector2 paddle1Pos;              // Posição do paddle1 (atualizada pelo servidor)
    public Vector2 paddle2Pos;              // Posição do paddle2 (atualizada pelo servidor)

    //public UdpServerPong server;          // Referência ao servidor para enviar posição

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Direção inicial aleatória
        direction = new Vector2(Random.value < 0.5f ? -1 : 1, Random.Range(-0.5f, 0.5f)).normalized;

        // Define velocidade inicial
        rb.linearVelocity = direction * speed;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        SendBallPosition();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // --- COLISÃO COM PAREDES LATERAIS (pontuação) ---
        if (collision.gameObject.CompareTag("WallInimigo"))
        {
            // Bola atingiu a parede do inimigo → ponto pro servidor
            GameManager.Instance.AddMyPoint();
            ResetBall();
        }
        else if (collision.gameObject.CompareTag("MyWall"))
        {
            // Bola atingiu a parede do servidor → ponto pro cliente
            GameManager.Instance.AddEnemyPoint();
            ResetBall();
        }

        // --- PAREDES SUPERIOR E INFERIOR ---
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Rebote vertical
            if (collision.contacts[0].normal.y != 0)
                direction = new Vector2(direction.x, -direction.y);
            rb.linearVelocity = direction * speed;
        }

        // --- RAQUETES ---
        else if (collision.gameObject.CompareTag("Paddle") || collision.gameObject.CompareTag("PaddleClient"))
        {
            // Rebote horizontal
            direction = new Vector2(-direction.x, direction.y);

            // Adiciona leve variação conforme o ponto de contato
            float offset = transform.position.y - collision.transform.position.y;
            direction.y = offset * 2f;
            direction.Normalize();

            rb.linearVelocity = direction * speed;
        }
    }

    void ResetBall()
    {
        // Reposiciona a bola no centro e relança com direção aleatória
        transform.position = Vector2.zero;
        direction = new Vector2(Random.value < 0.5f ? -1 : 1, Random.Range(-0.5f, 0.5f)).normalized;
        rb.linearVelocity = direction * speed;
    }

    void SendBallPosition()
    {
        string msg = $"BALL:{transform.position.x.ToString("F2", CultureInfo.InvariantCulture)};" +
                     $"{transform.position.y.ToString("F2", CultureInfo.InvariantCulture)}";
        //server.BroadcastToAllClients(msg);
    }
}