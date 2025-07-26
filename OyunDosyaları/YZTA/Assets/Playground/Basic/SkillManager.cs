using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoBehaviour
{
    [Header("Skill References")]
    public Korkusanlar skill1;
    public Korkusanlar skill2;
    public Korkusanlar skill3;

    [Header("Cooldown Durations (seconds)")]
    public float cooldown1 = 5f;
    public float cooldown2 = 5f;
    public float cooldown3 = 5f;

    private InputAction action1;
    private InputAction action2;
    private InputAction action3;

    private float cooldownTimer1 = 0f;
    private float cooldownTimer2 = 0f;
    private float cooldownTimer3 = 0f;

    void OnEnable()
    {
        action1 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
        action2 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/2");
        action3 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/3");

        action1.performed += ctx => TryActivateSkill(1);
        action2.performed += ctx => TryActivateSkill(2);
        action3.performed += ctx => TryActivateSkill(3);

        action1.Enable();
        action2.Enable();
        action3.Enable();
    }

    void OnDisable()
    {
        action1.performed -= ctx => TryActivateSkill(1);
        action2.performed -= ctx => TryActivateSkill(2);
        action3.performed -= ctx => TryActivateSkill(3);

        action1.Disable();
        action2.Disable();
        action3.Disable();
    }

    void Update()
    {
        if (cooldownTimer1 > 0f) cooldownTimer1 -= Time.deltaTime;
        if (cooldownTimer2 > 0f) cooldownTimer2 -= Time.deltaTime;
        if (cooldownTimer3 > 0f) cooldownTimer3 -= Time.deltaTime;
    }

    private void TryActivateSkill(int skillNumber)
    {
        switch (skillNumber)
        {
            case 1:
                if (cooldownTimer1 <= 0f && skill1 != null)
                {
                    skill1.FIRE = true;
                    cooldownTimer1 = cooldown1;
                    Debug.Log("Skill 1 activated");
                }
                else
                {
                    Debug.Log($"Skill 1 is on cooldown. Remaining time: {cooldownTimer1:F2} seconds");
                }
                break;

            case 2:
                if (cooldownTimer2 <= 0f && skill2 != null)
                {
                    skill2.FIRE = true;
                    cooldownTimer2 = cooldown2;
                    Debug.Log("Skill 2 activated");
                }
                else
                {
                    Debug.Log($"Skill 2 is on cooldown. Remaining time: {cooldownTimer2:F2} seconds");
                }
                break;

            case 3:
                if (cooldownTimer3 <= 0f && skill3 != null)
                {
                    skill3.FIRE = true;
                    cooldownTimer3 = cooldown3;
                    Debug.Log("Skill 3 activated");
                }
                else
                {
                    Debug.Log($"Skill 3 is on cooldown. Remaining time: {cooldownTimer3:F2} seconds");
                }
                break;
        }
    }
}
