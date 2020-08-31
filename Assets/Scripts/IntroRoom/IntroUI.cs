using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;

// Disable the "Field not assigned" warnings
#pragma warning disable 0649

public class IntroUI : MonoBehaviour
{
    [SerializeField] private Transform Table;
    private int Difficulty = 1;
    [SerializeField] GameObject[] DifficultyBlocks = new GameObject[8];
    [SerializeField] TextFade[] FadeTexts = new TextFade[3];
    [SerializeField] ImageFade[] FadeImages = new ImageFade[4];
    [SerializeField] private ImageFade SceneTransitionImage;
    [SerializeField] private ChessKingIntro chessKing;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeUI());
    }

    private IEnumerator FadeUI()
    {
        yield return new WaitForSecondsRealtime(1f);
        FadeTexts[0].FadeIn(1f);
        yield return new WaitForSecondsRealtime(3f);
        FadeTexts[0].FadeOut(0.5f);
        yield return new WaitForSecondsRealtime(1f);
        FadeTexts[1].FadeIn(1f);
        FadeTexts[2].FadeIn(1f);
        FadeImages[0].FadeIn(1f);
        FadeImages[1].FadeIn(1f);
        FadeImages[2].FadeIn(1f);
        FadeImages[3].FadeIn(1f);
    }

    private IEnumerator StartNextScene()
    {
        SceneTransitionImage.FadeIn(1f);
        yield return new WaitForSecondsRealtime(1.5f);

        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Side");

        while (async.progress < 0.9f)
        {
            yield return null;
        }
    }

    public void ChangeTable(string direction)
    {
        if (direction == "up")
        {
            if (Data.TableHeight < Data.maxTableHeight)
            {
                chessKing.Unlock();
                Table.transform.position += Vector3.up * Data.MoveTableFactor;
                Data.TableHeight++;
                chessKing.Lock();
            }
        }
        else if (direction == "down")
        {
            if (Data.TableHeight > Data.minTableHeight)
            {
                chessKing.Unlock();
                Table.transform.position -= Vector3.up * Data.MoveTableFactor;
                Data.TableHeight--;
                chessKing.Lock();
            }
        }
    }

    public void IncreaseDifficulty()
    {
        if (Difficulty < 8)
        {
            Difficulty++;
            AddDifficultyBlock();
        }
    }

    public void DecreaseDifficulty()
    {
        if(Difficulty > 1)
        {
            RemoveDifficultyBlock();
            Difficulty--;
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartNextScene());
    }

    private void RemoveDifficultyBlock()
    {
        DifficultyBlocks[Difficulty - 1].SetActive(false);
    }

    private void AddDifficultyBlock()
    {
        DifficultyBlocks[Difficulty - 1].SetActive(true);
    }
}
