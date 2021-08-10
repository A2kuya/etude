using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InteractManager interactManager;
    public RectTransform option;
    bool isPause = true;
    public Player player;
    public static GameManager Instance;
    public SaveData save;
    void Awake()
    {
        count = 1;
        if(Instance == null){
            Instance = this;
            save = new SaveData();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnLoadScene;
            player = GameObject.Find("Player").GetComponent<Player>();
            interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();          
        }else{
            Destroy(gameObject);
            return;
        }
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPause){
                Pause();
            }else{
                Resume();
            }
        }
    }
    private void OnLoadScene(Scene scene, LoadSceneMode mode){
        player = GameObject.Find("Player").GetComponent<Player>();
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
        player.canGetKey = true;
        option.anchoredPosition = new Vector3(0, 1000, 0);
    }
    public void Save(){
		save.hp = player.hp;
        save.money = player.money;
		save.skillPoint = player.skillPoint;
		save.positionX = player.transform.position.x;
		save.positionY = player.transform.position.y;
		save.scene = SceneManager.GetActiveScene().name;
        save.skillLevel[(int)SkillManager.SkillType.Dash] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.Dash];
        save.skillLevel[(int)SkillManager.SkillType.DoubleJump] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.DoubleJump];
        save.skillLevel[(int)SkillManager.SkillType.SpecialAttack] = SkillManager.Instance.skillLevel[(int)SkillManager.SkillType.SpecialAttack];
		SaveManager.Save(save, "test");
        SaveManager.Save(save, "auto");
	}
    public void Load(string s){
		save = SaveManager.Load(s);
		SceneManager.LoadScene(save.scene);
        Resume();
		save.scene = SceneManager.GetActiveScene().name;
	}

    public void AutoSave(){
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
    
    

}