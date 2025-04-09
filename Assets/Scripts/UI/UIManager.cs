using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  [Header("Buttons")]
  [SerializeField] private Button Random_Button;
  [SerializeField] private Button Play_Button;
  [SerializeField] private Button AutoPlay_Button;
  [SerializeField] private Button StakePlus_Button;
  [SerializeField] private Button StakeMinus_Button;
  [SerializeField] private Button Reset_Button;
  [SerializeField] private Button Delete_Button;
  [SerializeField] private Button GameExit_Button;
  [SerializeField] private Button MaxPopup_Button;

  [Header("Texts")]
  [SerializeField] private TMP_Text Stake_Text;
  [SerializeField] private TMP_Text PopupWin_Text;
  [SerializeField] private TMP_Text Win_Text;
  [SerializeField] private TMP_Text TotalBet_text;

  [Header("Lists")]
  [SerializeField] private List<TMP_Text> Payout_Text;
  [SerializeField] private List<TMP_Text> Hits_Text;
  [SerializeField] private List<GameObject> Win_Objects;

  [Header("GameObjects")]
  [SerializeField] private GameObject Reset_Object;
  [SerializeField] private GameObject PlayAnim_Object;
  [SerializeField] private GameObject CoinValueDisable_object;
  [SerializeField] private GameObject StarAnim_Object;

  [Header("Scripts")]
  [SerializeField] private KenoBehaviour KenoManager;
  [SerializeField] private SocketIOManager socketManager;

  [Header("Popups")]
  [SerializeField] private GameObject MainPopup_Object;
  [SerializeField] private GameObject MaxPopup_Object;
  [SerializeField] private GameObject WinPopup_Object;
  [SerializeField] private Transform WinPopup_Transform;
  [SerializeField] private GameObject CoinAnim_Object;

  [Header("Image Animation Script")]
  [SerializeField] private ImageAnimation TitleAnim;

  private int stake = 5;
  internal bool isReset = false;

  void Start()
  {
    if (Random_Button) Random_Button.onClick.RemoveAllListeners();
    if (Random_Button) Random_Button.onClick.AddListener(PickRandomIndices);

    if (Play_Button) Play_Button.onClick.RemoveAllListeners();
    if (Play_Button) Play_Button.onClick.AddListener(PlayKeeno);

    CheckPlayButton(false);

    if (StakePlus_Button) StakePlus_Button.onClick.RemoveAllListeners();
    if (StakePlus_Button) StakePlus_Button.onClick.AddListener(delegate { ChangeStake(true); });

    if (StakeMinus_Button) StakeMinus_Button.onClick.RemoveAllListeners();
    if (StakeMinus_Button) StakeMinus_Button.onClick.AddListener(delegate { ChangeStake(false); });

    if (Reset_Button) Reset_Button.onClick.RemoveAllListeners();
    if (Reset_Button) Reset_Button.onClick.AddListener(ResetGame);

    if (Delete_Button) Delete_Button.onClick.RemoveAllListeners();
    if (Delete_Button) Delete_Button.onClick.AddListener(CleanButtons);

    // if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
    // if (GameExit_Button) GameExit_Button.onClick.AddListener(CallOnExitFunction);    //TODO: Uncomment this line and change the function logic.

    if (MaxPopup_Button) MaxPopup_Button.onClick.RemoveAllListeners();
    if (MaxPopup_Button) MaxPopup_Button.onClick.AddListener(MaxPopupDisable);

    stake = 5;
    if (Stake_Text) Stake_Text.text = stake.ToString();
    if (TotalBet_text) TotalBet_text.text = stake.ToString();
    //if (Win_Text) Win_Text.text = winning.ToString();
    // Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
  }

  // private void CallOnExitFunction()
  // {
  //   Application.ExternalCall("window.parent.postMessage", "onExit", "*");
  // }

  private void PlayKeeno()
  {
    if (StarAnim_Object) StarAnim_Object.SetActive(true);
    if (isReset)
    {
      ResetGame();
    }
    isReset = true;
    CheckPlayButton(false);
    if (Delete_Button) Delete_Button.interactable = false;
    if (CoinValueDisable_object) CoinValueDisable_object.SetActive(true);
    KenoManager.PlayKeeno();
    DOVirtual.DelayedCall(0.5f, () =>
    {
      if (StarAnim_Object) StarAnim_Object.SetActive(false);
    });
  }

  private void ChangeStake(bool type)
  {
    if (type)
    {
      KenoManager.betCounter++;
      if (KenoManager.betCounter >= socketManager.initialData.Bets.Count)
      {
        KenoManager.betCounter = 0;
      }
    }
    else
    {
      KenoManager.betCounter--;
      if (KenoManager.betCounter < 0)
      {
        KenoManager.betCounter = socketManager.initialData.Bets.Count - 1;
      }
    }
    if (Stake_Text) Stake_Text.text = socketManager.initialData.Bets[KenoManager.betCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.Bets[KenoManager.betCounter].ToString();
    UpdateSelectedText();
  }

  internal void initGame(){
    UpdateSelectedText();
    if (Stake_Text) Stake_Text.text = socketManager.initialData.Bets[0].ToString();
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.Bets[0].ToString();
  }

  private void PickRandomIndices()
  {
    if (isReset)
    {
      ResetGame();
    }
    KenoManager.PickRandoms();
  }

  internal void CheckPlayButton(bool isActive)
  {
    if (Play_Button) Play_Button.interactable = isActive;
    if (Random_Button) Random_Button.interactable = isActive;
    if (AutoPlay_Button) AutoPlay_Button.interactable = isActive;
    if (PlayAnim_Object) PlayAnim_Object.SetActive(isActive);
  }

  private void WinPopupEnable()
  {
    CancelInvoke("WinPopupDisable");
    if(PopupWin_Text) PopupWin_Text.text = socketManager.playerdata.currentWining.ToString("F2");
    if (TitleAnim) TitleAnim.StartAnimation();
    if (WinPopup_Transform) WinPopup_Transform.localScale = Vector3.zero;
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
    if (WinPopup_Object) WinPopup_Object.SetActive(true);
    if (WinPopup_Transform) WinPopup_Transform.DOScale(Vector3.one, 0.5f);
    if (CoinAnim_Object) CoinAnim_Object.SetActive(true);
    Invoke("WinPopupDisable", 5f);
  }

  private void WinPopupDisable()
  {
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    if (WinPopup_Object) WinPopup_Object.SetActive(false);
    if (CoinAnim_Object) CoinAnim_Object.SetActive(false);
  }

  internal void MaxPopupEnable()
  {
    CancelInvoke("MaxPopupDisable");
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
    if (MaxPopup_Object) MaxPopup_Object.SetActive(true);
    Invoke("MaxPopupDisable", 2f);
  }

  private void MaxPopupDisable()
  {
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    if (MaxPopup_Object) MaxPopup_Object.SetActive(false);
  }

  internal void UpdateSelectedText()
  {
    BetAmountUpdate();
  }

  internal void CheckFinalWinning()
  {
    if(socketManager.playerdata.currentWining>0){
      WinningsTextUpdate(socketManager.playerdata.currentWining);
      WinPopupEnable();
    }
  }

  internal void WinningsTextUpdate(double amount)
  {
    // Debug.Log("Here");
    if (Win_Text) Win_Text.text = amount.ToString("F2");
  }

  void BetAmountUpdate()
  {

    for (int i = 0; i < Payout_Text.Count; i++)
    {
      if (Payout_Text[i]) Payout_Text[i].text = string.Empty;
    }

    for (int i = 0; i < Hits_Text.Count; i++)
    {
      if (Hits_Text[i]) Hits_Text[i].text = string.Empty;
    }

    if (KenoManager.selectionCounter <= 1)
    {
      Hits_Text[0].text = "1";
      Payout_Text[0].text = (socketManager.initialData.Paytable[0][0] * socketManager.initialData.Bets[KenoManager.betCounter]).ToString("F2");
    }
    else
    {
      for (int i = 0; i < KenoManager.selectionCounter; i++)
      {
        if (Hits_Text[i]) Hits_Text[i].text = (i + 1).ToString();
      }
      for(int i=0;i<socketManager.initialData.Paytable[KenoManager.selectionCounter-1].Count;i++)
      {
        if (Payout_Text[i]) Payout_Text[i].text = (socketManager.initialData.Paytable[KenoManager.selectionCounter-1][i] * socketManager.initialData.Bets[KenoManager.betCounter]).ToString("F2");  
      }
    }
  }

  internal void EnableReset()
  {
    if (Reset_Object) Reset_Object.SetActive(true);
    if (Delete_Button) Delete_Button.interactable = true;
    if (CoinValueDisable_object) CoinValueDisable_object.SetActive(false);
    CheckPlayButton(true);
  }

  private void ResetGame()
  {
    KenoManager.ResetWinAnim();
    if (TitleAnim) TitleAnim.StopAnimation();
    KenoManager.ResetButtons();
    if (Reset_Object) Reset_Object.SetActive(false);
    isReset = false;
    WinningsTextUpdate(0);
  }

  private void CleanButtons()
  {
    // BetAmountUpdate(0);
    UpdateSelectedText();
    WinningsTextUpdate(0);
    KenoManager.CleanPage();
    CheckPlayButton(false);
  }

}
