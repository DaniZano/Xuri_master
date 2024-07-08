using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f; // Moltiplicatore per aumentare la velocità di caduta
    public Animator animator;
    private Rigidbody rb;
    private float currentSpeed;
    private bool isGrounded;
    private bool isJumping;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Congela la rotazione per evitare ribaltamenti
        currentSpeed = walkSpeed; // Imposta la velocità iniziale come la velocità di camminata
    }

    void Update()
    {
        // Controlla se il tasto "E" è premuto per cambiare la velocità e lo stato dell'animazione
        if (Input.GetKey(KeyCode.E))
        {
            currentSpeed = runSpeed;
            animator.SetBool("isRunning", true);
        }
        else
        {
            currentSpeed = walkSpeed;
            animator.SetBool("isRunning", false);
        }

        // Controlla se il tasto "spazio" è premuto per saltare
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isJumping = true;
        }
    }

    void FixedUpdate() // Usare FixedUpdate per la fisica
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        Vector3 movement = new Vector3(moveHorizontal * currentSpeed * Time.fixedDeltaTime, 0, 0);
        rb.MovePosition(transform.position + movement);

        // Aggiungi gravità extra durante la caduta
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (moveHorizontal != 0)
        {
            animator.SetBool("isWalking", true);

            if (moveHorizontal > 0)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0); // Destra
            }
            else if (moveHorizontal < 0)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0); // Sinistra
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            Debug.Log("isWalking: false");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Controlla se il giocatore è a terra
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false; // Il giocatore è nuovamente a terra dopo il salto
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Controlla se il giocatore non è più a terra
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
