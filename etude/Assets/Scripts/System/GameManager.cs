using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InteractManager interactManager;
    public RectTransform option;
    public RectTransform setting;
    public bool isFullScreen;

    bool isPause = true;
    public Player player;
    public MonsterFactory monsterFactory;
    public BossFactory bossFactory;
    public static GameManager Instance;
    public SaveData save;
    void Awake()
    {
        count = 1;
        if(Instance == null){
            save = null;
            Instance = this;
            monsterFactory = new MonsterFactory();
            bossFactory = new BossFactory();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLoadScene;
            if (GameObject.Find("Player"))
                player = GameObject.Find("Player").GetComponent<Player>();
            if (GameObject.Find("InteractManager"))
                interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();          
            if(player)
                AutoSave();
        }else{
            Destroy(gameObject);
            return;
        }
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if (GameObject.Find("Player"))
            {
                if (isPause)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }
    }
    private void OnLoadScene(Scene scene, LoadSceneMode mode){
        if (GameObject.Find("Player"))
            player = GameObject.Find("Player").GetComponent<Player>();
        if (GameObject.Find("InteractManager"))
            interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();
    }
    public void Action(GameObject scanObj)
    {
        interactManager.Action(scanObj);
    }

    //Endless System
    private int count;
    public void addCount()
    {
        count++;
    }
    public int getCount()
    {
        return count;
    }

    public void setCount(int a)
    {
        count=a;
    }

    public void Pause(){
        Time.timeScale = 0;
        isPause = false;
        player.canGetKey = false;
        option.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void Resume(){ 
        Time.timeScale = 1;
        isPause = true;
        if (GameObject.Find("Player"))
            player.canGetKey = true;
        option.anchoredPosition = new Vector3(0, 1000, 0);
    }
    public void Save(){
        save = new SaveData();
		save.hp = player.hp;
        save.money = player.money;
		save.skillPoint = player.skillPoint;
		save.positionX = player.transform.position.x;
		save.positionY = player.transform.position.y;
		save.scene = SceneManager.GetActiveScene().name;
        save.skillLevel[(int)SkillManager.SkillType.Dash] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.Dash];
        save.skillLevel[(int)SkillManager.SkillType.DoubleJump] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.DoubleJump];
        save.skillLevel[(int)SkillManager.SkillType.SpecialAttack] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.SpecialAttack];
        SaveManager.Save(save, "auto");
        SaveManager.Save(save, "test");
    }
    public void Load(string s){
		save = SaveManager.Load(s);
		SceneManager.LoadScene(save.scene);
        Resume();
		save.scene = SceneManager.GetActiveScene().name;
	}

    public void AutoSave(){
        save = new SaveData();
        save.hp = player.hp;
        save.money = player.money;
		save.skillPoint = player.skillPoint;
		save.positionX = player.transform.position.x;
		save.positionY = player.transform.position.y;
		save.scene = SceneManager.GetActiveScene().name;
        save.skillLevel[(int)SkillManager.SkillType.Dash] = 0;
        save.skillLevel[(int)SkillManager.SkillType.Dash] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.Dash];
        save.skillLevel[(int)SkillManager.SkillType.DoubleJump] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.DoubleJump];
        save.skillLevel[(int)SkillManager.SkillType.SpecialAttack] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.SpecialAttack];
		SaveManager.Save(save, "auto");
    }

    public void LoadScene(string s, Vector2 position){
        AutoSave();
        save.positionX = position.x;
        save.positionY = position.y;
        SceneManager.LoadScene(s);        
    }

    public void Setting()
    {
        option.anchoredPosition = new Vector3(0, 1000, 0);
        setting.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void Back()
    {
        option.anchoredPosition = new Vector3(0, 0, 0);
        setting.anchoredPosition = new Vector3(0, 1000, 0);
    }

    public void SetFullScreen(bool toggle)
    {
        isFullScreen = toggle;
        Screen.fullScreen = toggle;
    }

    public void SetRatio(int num)
    {
        switch(num)
        {
            case 0:
                Screen.SetResolution(800, 600, isFullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, isFullScreen);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
            case 3:
                Screen.SetResolution(3840, 2160, isFullScreen);
                break;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}