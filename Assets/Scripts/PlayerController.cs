using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 9.0f;
    public float gravity = -19.62f; // Gravedad realista
    public float jumpHeight = 1.5f;

    [Header("Cámara y Ratón")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2.0f;
    public float topLookLimit = 85.0f;
    public float bottomLookLimit = -85.0f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Bloquear y ocultar el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Buscar la cámara automáticamente si no se asignó en el Inspector
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        LookAround();
        MovePlayer();
    }

    void LookAround()
    {
        if (cameraTransform == null) return;

        // Capturar movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 50f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 50f * Time.deltaTime;

        // Rotación vertical (Cámara)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, bottomLookLimit, topLookLimit);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (Cuerpo del Jugador)
        transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        // Verificar si está tocando el suelo
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Mantener al jugador pegado al suelo
        }

        // Entradas del teclado (WASD / Flechas)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Chequear si presiona Shift para sprintar
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Calcular dirección de movimiento según la orientación del jugador
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Salto (Tecla Espacio)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}