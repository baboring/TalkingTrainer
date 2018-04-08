using UnityEngine;
using UnityEngine.UI;


namespace TalkTrainer.Logic
{
    using Common.Utilities;
    using NaughtyAttributes;

    public enum BoardCmd {
        None,
        FlagOn,
        FlagOff,
        Confirm,
    }

    public class CellInfo {

        public bool isMine = false;
        public bool isCovered = true;
        public bool isFlaged = false;
        public bool isExploded = false;
        public int mineCount = 0;


        // Try to confirm cell whether explosion or not
        public bool Open() {

            isCovered = false;

            if(!IsVaild) {
                isExploded = true;
                return false;
            }

            return true;
        }

        public bool IsVaild
        {
            get
            {
                if (isMine && isFlaged)
                    return true;

                if (isMine)
                    return false;

                return true;
            }
        }

        public bool IsExplosion
        {
            get {
                return !isCovered && isMine && !isFlaged;
            }
        }
    }

    public class MineCell : Button
    {
        [System.NonSerialized]
        public int m_idx;

        public CellInfo Info {
            get; private set;
        }

        [SerializeField]
        protected Image imageTarget;

        [SerializeField]
        protected Image mark;

        [SerializeField]
        protected Sprite spriteBlank;
        [SerializeField]
        protected Sprite spriteCovered;
        [SerializeField]
        protected Sprite spriteFlag;
        [SerializeField]
        protected Sprite spriteGrayMine;
        [SerializeField]
        protected Sprite spriteRedMine;

        [ReorderableList]
        [SerializeField]
        protected Sprite[] spriteNumber;

        public void Initial(int cellIdx)
        {
            m_idx = cellIdx;
            Info = new CellInfo();
            this.gameObject.SetActive(true);
        }

        bool m_isLastButtonDown;
        bool m_isButtonDown;
        float m_elapsedtimePressed;

        public void OnClicked()
        {
            Logger.Debug("Click:" + m_idx);
            if (m_isLastButtonDown != m_isButtonDown)
                return;
            if(Info.isFlaged)
                BoardController.instance.Invoke(BoardCmd.FlagOff, m_idx);
            else if(Info.isCovered)
                BoardController.instance.Invoke(BoardCmd.Confirm, m_idx);

        }

        public void OnHoldDown()
        {
            Debug.Log("OnHoldDown:" + m_idx + "," + m_isButtonDown);
            BoardController.instance.Invoke(
                Info.isFlaged? BoardCmd.FlagOff:BoardCmd.FlagOn, m_idx);
        }

        public void Refresh()
        {
            // state covered
            if(Info.isCovered) {
                imageTarget.sprite = spriteCovered;

                if (Info.isFlaged) {
                    mark.sprite = spriteFlag;
                }
                else {
                    mark.sprite = null;
                }
            }
            else {

                imageTarget.sprite = spriteBlank;

                if(!Info.IsVaild) {
                    mark.enabled = true;
                    mark.sprite = spriteRedMine;
                }
                else if (Info.isMine)
                {
                    mark.enabled = true;
                    mark.sprite = spriteGrayMine;
                }
                else if(Info.mineCount > 0) {
                    mark.enabled = true;
                    mark.sprite = spriteNumber[Info.mineCount];
                }

            }

            if(Info.isExploded)
                mark.sprite = spriteRedMine;

            mark.enabled = mark.sprite != null;
        }

        public void Update()
        {
            if (IsPressed())
            {
                if (!m_isButtonDown) {
                    m_isLastButtonDown = m_isButtonDown = true;
                    m_elapsedtimePressed = 0;
                    Logger.Debug("Pressed:" + m_idx + "," + m_isButtonDown);
                }
                if(m_isButtonDown == m_isLastButtonDown)
                    m_elapsedtimePressed += Time.deltaTime;
                //onClick.Invoke();
            }
            else {
                if (m_isButtonDown) {
                    m_isLastButtonDown = m_isButtonDown = false;
                    Logger.Debug("Pressed:" + m_idx + "," + m_isButtonDown);
                }

            }

            if (m_elapsedtimePressed > 1) {
                m_isLastButtonDown = !m_isButtonDown;
                m_elapsedtimePressed = 0;
                OnHoldDown();
            }
        }
	}
}