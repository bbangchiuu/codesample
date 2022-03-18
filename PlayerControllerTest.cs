using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTest : MonoBehaviour
{
    PlayerActionsAssetTest playerActions;
    private CharacterController controller;
    Vector3 currentMovement;

    private float playerSpeed;
    private float playerRun = 5.0f;
    private float playerWalk = 1.0f;
    private float playerDash = 9.0f;
    private float _dashTime = 0.1f;
    private bool midAirJump = false;

    private float gravityValue = 9.81f;
    [SerializeField]
    bool isGrounded;
    [SerializeField]
    bool isClimb;

    [SerializeField]
    Transform cameraMain;
    private float jumpSpeed = 3.5f;
    [SerializeField]
    private float _directionY;

    [SerializeField]
    Transform groundedCheck;
    LayerMask layerGround;
    LayerMask layerWall;
    [SerializeField]
    bool checkClim;
    [SerializeField]
    bool climbFinish;
    [SerializeField]
    Transform playerHeader;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerActions = GetComponent<PlayerActionsAssetTest>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerRun;
        layerGround = LayerMask.GetMask("Ground");
        layerWall = LayerMask.GetMask("Wall");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        GestCollision();
        HandleMove();
        HandleClimb();
        HandleJump();
        //move and dash
        controller.Move(currentMovement * Time.deltaTime * playerSpeed);
        if (playerActions.isHoldDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    void HandleClimb()
    {
        if (!checkClim)
        {
            return;
        }

        playerSpeed = 10f;
        currentMovement = new Vector3(playerActions.move.x, playerActions.move.y, 0);
    }

    void GestCollision()
    {
        isGrounded = Physics.Raycast(groundedCheck.position, Vector3.down, 0.5f, layerGround);
        checkClim = Physics.Raycast(transform.position, transform.forward, 0.7f, layerWall);
        climbFinish = Physics.Raycast(playerHeader.position, transform.forward, 0.5f, layerWall);
    }
    void HandleMove()
    {
        currentMovement = new Vector3(playerActions.move.x, 0, playerActions.move.y);
        currentMovement = cameraMain.forward * currentMovement.z + cameraMain.right * currentMovement.x;
        if (currentMovement != Vector3.zero)
        {
            if (playerActions.stateMove == PlayerActionsAssetTest.StateMove.Run)
            {
                playerSpeed = playerRun;
            }
            else if (playerActions.stateMove == PlayerActionsAssetTest.StateMove.Walk)
            {
                playerSpeed = playerWalk;
            }

            transform.forward = currentMovement;
            //float rote = Mathf.Rad2Deg * Mathf.Atan2(playerActions.movement.x, playerActions.movement.y) + cameraMain.eulerAngles.y;
            float rote = transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, rote, 0);
        }
    }

    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time; // need to remember this to know how long to dash
        while (Time.time < startTime + _dashTime)
        {
            controller.Move(transform.forward * playerDash * Time.deltaTime);
            // or controller.Move(...), dunno about that script
            yield return null; // this will make Unity stop here and continue next frame
        }
    }

    void HandleJump()
    {
        if (isClimb || checkClim)
        {
            return;
        }

        if (playerActions.OnJump() && isGrounded)
        {
            midAirJump = true; 
            _directionY = jumpSpeed;

        }
        else if(playerActions.OnJump() && midAirJump)
        {
            _directionY = jumpSpeed;
            midAirJump = false;
        }
        
        _directionY -= gravityValue * Time.deltaTime;
        if(_directionY <= -Mathf.Abs(gravityValue))
        {
            _directionY = 0;
        }
        currentMovement.y = _directionY;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PlacementTile placementTile = hit.gameObject.GetComponent<PlacementTile>();
        if(placementTile != null)
        {
            placementTile.SelfDestroy();
        }
    }
}
