using Unity.Netcode;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
namespace NetcodeDemo
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField] ThirdPersonController m_ThirdPersonController;
        [SerializeField] PlayerInput m_PlayerInput;
        [SerializeField] StarterAssetsInputs m_StarterAssetsInputs;
        private void Awake()
        {
            m_PlayerInput.enabled = false;
            m_StarterAssetsInputs.enabled = false;
            m_ThirdPersonController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
            m_PlayerInput.enabled = true;
            m_ThirdPersonController.enabled = true;
            m_StarterAssetsInputs.enabled = true;
            }
        
        }
    }
}