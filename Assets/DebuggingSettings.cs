using UnityEngine;
using RuntimeDebugUI;


public class DebuggingSettings : DebugUI
{
    public CharacterController controller;

    protected override void ConfigureTabs()
    {
        AddTab(ConfigurePlayerTab());

    }

    private DebugTabConfig ConfigurePlayerTab()
    {
        DebugTabConfig tab = new DebugTabConfig()
        {
            name = "Player",
            displayName = "Player Settings"
        };

        tab.controls.Add(new DebugControlConfig
        {
            name = "MoveSpeed",
            displayName = "Move Speed",
            tooltip = "Player Movement Speed",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 5.0f,
            maxValue = 50.0f,
            getter = () => controller.baseSpeed,
            setter = (value) =>
            {
                controller.baseSpeed = value; //set the move speed from the appropriate object }
            }
        });

        return tab;
    }


    private void Awake()
    {
        controller = FindAnyObjectByType<CharacterController>();

        if (controller != null) Debug.Log("Reference Filled");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
