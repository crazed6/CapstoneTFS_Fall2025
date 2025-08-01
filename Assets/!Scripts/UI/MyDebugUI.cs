using UnityEngine;
using RuntimeDebugUI;
using Cinemachine;

public class MyDebugUI : DebugUI
{

    [SerializeField] private CinemachineFreeLook freelookCam;

    private void Awake()
    {
        freelookCam = GameObject.FindGameObjectWithTag("FreeLook").GetComponent<CinemachineFreeLook>();
    }

    protected override void Start()
    {
        base.Start();
    }
    protected override void ConfigureTabs()
    {
        AddTab(CinemachineSettings());
    }

    private DebugTabConfig CinemachineSettings()
    {
        DebugTabConfig tabConfig = new DebugTabConfig
        {
            name = "Cinemachine",
            displayName = "Cinemachine Settings"
        };
        
        tabConfig.controls.Add(new DebugControlConfig
        {
            sectionName = "Top Orbit",
            name = "TopOrbitHeight",
            displayName = "Top Orbit Height",
            tooltip = "This is the orbit settings for the top ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[0].m_Height,
            setter = (value) => freelookCam.m_Orbits[0].m_Height = value
        });
        tabConfig.controls.Add(new DebugControlConfig
        {
            sectionName = "Top Orbit",
            name = "TopOrbitRadius",
            displayName = "Top Orbit Radius",
            tooltip = "This is the orbit settings for the top ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[0].m_Radius,
            setter = (value) => freelookCam.m_Orbits[0].m_Radius = value
        });

        tabConfig.controls.Add(new DebugControlConfig
        {
            sectionName = "Middle Orbit",
            name = "MiddleOrbitRadius",
            displayName = "Middle Orbit Radius",
            tooltip = "This is the orbit settings for the middle ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[1].m_Radius,
            setter = (value) => freelookCam.m_Orbits[1].m_Radius = value
        });

        tabConfig.controls.Add(new DebugControlConfig
        {

            sectionName = "Middle Orbit",
            name = "MiddleOrbitHeight",
            displayName = "Middle Orbit Height",
            tooltip = "This is the orbit settings for the middle ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[1].m_Height,
            setter = (value) => freelookCam.m_Orbits[1].m_Height = value
        });


        tabConfig.controls.Add(new DebugControlConfig
        {
            sectionName = "Bottom Orbit",
            name = "BottomOrbitRadius",
            displayName = "Bottom Orbit Radius",
            tooltip = "This is the orbit settings for the bottom ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[2].m_Radius,
            setter = (value) => freelookCam.m_Orbits[2].m_Radius = value
        });

        tabConfig.controls.Add(new DebugControlConfig
        {
            sectionName = "Bottom Orbit",
            name = "BottomOrbitHeight",
            displayName = "Bottom Orbit Height",
            tooltip = "This is the orbit settings for the bottom ring",
            type = DebugControlConfig.ControlType.Slider,
            saveValue = true,
            minValue = 0f,
            maxValue = 360f,
            getter = () => freelookCam.m_Orbits[2].m_Height,
            setter = (value) => freelookCam.m_Orbits[2].m_Height = value
        });

        return tabConfig;
    }
}
