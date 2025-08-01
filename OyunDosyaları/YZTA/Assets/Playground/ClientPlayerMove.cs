using Unity.Netcode;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField]
    OnlineCharacterController m_OnlineCharacterController;

    [SerializeField]
    PlayerInput m_PlayerInput;


    private void Awake()
    {
        m_PlayerInput.enabled = false;
        m_OnlineCharacterController.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        enabled = IsClient; // Enable if this is a client.
        if (!IsOwner)
        {
            // Disable if this is not the owner
            enabled = false;
            m_PlayerInput.enabled = false;
            m_OnlineCharacterController.enabled = false;
            return;
        }

        // Enable if this is an owner
        m_PlayerInput.enabled = true;
        m_OnlineCharacterController.enabled = true;

    }
}
