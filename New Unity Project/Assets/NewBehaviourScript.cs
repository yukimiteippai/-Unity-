using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//Text���������߂ɗv�ǉ�
using UnityEngine.SceneManagement;//LoadScene���g�����߂ɒǉ�
using System.IO;//�t�@�C���̏����o���֌W


public class NewBehaviourScript : MonoBehaviour
{
    GameObject[] cubes;
    GameObject canvas;  //Ganvas�I�u�W�F�N�g�擾�p
    Text text1;         //Text1�R���|�[�l���g�擾�p
    TextIntensity ti;   //TextIntensity�R���|�[�l���g�擾�p

    string str1 = "������m�o���Ă����\n�X�y�[�X�����������Ă�������\n�iesc�Ŏ����������I���j";
    string str2 = "�㉺�̖��ŋ��x����͂�Enter�L�[������";

    static LogSave csv = null;

    float timeFirstPush = 0f;//�ŏ��̐����̌�A���[�U��space�L�[�������͂��߂�����
    float timeFirstMove = 0f;//���̂��ړ����n�߂鎞��
    float timeAllPush = 0f;//space�L�[��������������

    void Start()
    {
        if (csv == null) csv = new LogSave();//new�����ƃt�@�C��������������Ă��܂��̂Ŕ�����
        timeFirstPush = 0f;//���Ԃ����������Ă���
        timeFirstMove = 0f;
        timeAllPush = 0f;

        cubes = new GameObject[500];
        float radi = 2.0f;
        float csize = 0.1f;

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].name = "Cube" + i.ToString();
            float xx = Random.Range(-1f * radi, 1f * radi);
            float yy = Random.Range(-1f * radi, 1f * radi);
            //float zz = Random.Range(Camera.main.transform.position.z, 0f);
            //��z=�J������Z�ʒu(-30��Unity��Őݒ�)�`0
            //�ȈՓI�ɏd�Ȃ��h���FZ�l�̈ʒu�����炷
            float dz = Camera.main.transform.position.z / cubes.Length;
            float zz = Random.Range(dz * i, dz * (i + 1) - csize);
            cubes[i].transform.position = new Vector3(xx, yy, zz);
            cubes[i].transform.localScale = new Vector3(csize, csize, csize);
            cubes[i].SetActive(false);
        }
        //Canvas���擾����canvas�ɑ��
        canvas = GameObject.Find("Canvas");
        //component�̎擾���q�K�w��Component���擾����
        text1 = canvas.GetComponentInChildren<Text>();
        ti = canvas.GetComponentInChildren<TextIntensity>();
        //�e�L�X�g�̐ݒ�
        text1.text = str1;// "������m�o���Ă����\n�X�y�[�X�����������Ă�������\n�iesc�Ŏ����������I���j";
        ti.setVisible(false);

        StartCoroutine(WaitProcess());
    }

    void Update()
    {
        //Enter�L�[�������ꂽ��
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //text��str2�ɂȂ�����(���l���͂��I��������)Enter���󂯕t����
            if (text1.text == str2)
            {
                SceneManager.LoadScene(0);

                //���Ԃ̕ۑ�                
                float timeToRecognize = timeFirstPush - timeFirstMove;
                if (timeFirstPush == 0f) timeToRecognize = -1;//�X�y�[�X�L�[�������Ȃ������ꍇ
                csv.logSave("," + timeToRecognize + "," + timeAllPush + "," + ti.getScore());
                //csv.logSave("Enter Key");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeFirstPush = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            timeAllPush = Time.time - timeFirstPush;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //�Q�l�Fhttps://web-dev.hatenablog.com/entry/unity/quit-game
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                UnityEngine.Application.Quit();
            #endif
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < cubes.Length; i++)
        {
            if (!cubes[i].activeSelf) continue;

            cubes[i].transform.Translate(0f, 0f, -0.1f);
            //cube���J���������ɍs������A�����ʒuz=0�ɖ߂�
            Vector3 cube = cubes[i].transform.position;
            if (cube.z < Camera.main.transform.position.z)
            {
                cubes[i].transform.position = new Vector3(cube.x, cube.y, 0f);
            }
        }
    }


    IEnumerator WaitProcess()
    {
        //Debug.Log("�w�肵���b������������҂��܂�");        
        yield return new WaitForSeconds(2f);

        Debug.Log("�҂��I���");
        for (int i = 0; i < cubes.Length; i++)
        {
            //cubes��Active�ɂ���
            cubes[i].SetActive(true);
        }     
        //Canvas���Active�ɂ���
        canvas.SetActive(false);

        //�����n�߂̎���
        timeFirstMove = Time.time;

        //        
        float t = 2f;//cubes�̈ړ�����
        yield return new WaitForSeconds(t);

        Debug.Log("cubes���ړ�������̏���");
        for (int i = 0; i < cubes.Length; i++)
        {
            //cubes���Active�ɂ���
            cubes[i].SetActive(false);
        }
        //Canvas��Active�ɂ���
        canvas.SetActive(true);
        //�\���e�L�X�g��ς���:
        text1.text = str2;// "�㉺�̖��ŋ��x����͂�Enter�L�[������";
        //text1.fontSize = 30;

        //�����łɔ�A�N�e�B�u���ƃA�N�Z�X���邾���ŃG���[�Hhttps://marunouchi-tech.i-studio.co.jp/2290/
        ti.setVisible(true);
    }
}



public class LogSave
{
    string filename;
    FileInfo fi;

    public LogSave()
    {
        System.DateTime thisDay = System.DateTime.Now;
        string day = //thisDay.Year.ToString() +
            thisDay.Month.ToString() +
            thisDay.Day.ToString() +
            "_" +
            thisDay.Hour.ToString() +
            thisDay.Minute.ToString();
        //filename = @"C:/Users/yuki/Documents/FileName" + day + ".csv";
        //�����̂悤�ɐ�΃p�X�Ńt�H���_�ʒu���w�肷�邱�Ƃ��ł���
        filename = "FileName" + day + ".csv";
        //��Assets�Ɠ����t�H���_�ɕۑ������B
        //Project�E�B���h�E����Assets���E�N���b�N��ShowInExplorer��I�Ԃƃt�H���_���J����

        fi = new FileInfo(filename);
        //fi = new FileInfo(Application.dataPath + "../FileName.csv");

        using (var sw = new StreamWriter(
                fi.Create(),
                System.Text.Encoding.UTF8))
        {
            sw.WriteLine("," + "�����n�߂Ă���{�^���������܂ł̎���" +
                            "," + "�{�^����������������" +
                            "," + "��ϓI���x");
        }
    }

    public void logSave(string txt)
    {
        //������t�@�C���ɒǉ��ŏ������ޏꍇ��AppendText
        using (StreamWriter sw = fi.AppendText())
        {
            sw.WriteLine(txt);
        }
    }

}