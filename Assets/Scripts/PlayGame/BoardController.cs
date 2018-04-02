using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TalkTrainer.Logic
{
    using Common.Utilities;
    using NaughtyAttributes;

    public class BoardController : MonoSingleton<BoardController>
    {
        [SerializeField]
        protected int totalMine = 10;

        [SerializeField]
        protected MineCell prefabCell;

        [Dropdown("boardTypes")]
        [SerializeField]
        protected Tuple<int, int> boardSize = new Tuple<int, int>(8, 8);

        private DropdownList<Tuple<int, int>> boardTypes = new DropdownList<Tuple<int, int>>() {
            {"8 x 8", new Tuple<int, int>(8, 8)},
            {"16 x 16", new Tuple<int, int>(16, 16)},
        };

        // list all cell
        protected MineCell[] m_lstCell = new MineCell[0];

        // list mines
        protected int[] m_lstMine = new int[0];

        [Button]
        public void GameOver() 
        {
            foreach (var obj in m_lstCell)  {
                obj.Info.Open();
                obj.Refresh();
            }

        }

        [Button]
        public void Restart()
        {
            ResetBoard();
            Build(boardSize);
        }


        // Reset Board ( remove all cells )
		private void ResetBoard()
		{
            // destroy
            foreach(var obj in m_lstCell) {
                DestroyImmediate(obj.gameObject);
            }
 
            m_lstCell = new MineCell[0];
            m_lstMine = new int[0];
		}

        // Build Mine board
        bool Build(Tuple<int, int> size) 
        {
            if (totalMine < 1)
                return false;
            
            m_lstCell = new MineCell[size.First * size.Second];
            m_lstMine = new int[totalMine];

            int idx = 0;
            for (int y = 0; y < size.Second;++y)
                for (int x = 0; x < size.First; ++x) {
                    var obj = Instantiate<MineCell>(prefabCell);
                    obj.transform.SetParent(this.transform);
                    obj.Initial(idx);
                    obj.name = string.Format("cell {0:000}",idx);
                    m_lstCell[idx] = obj;
                    ++idx;
                }


            System.Random random = new System.Random();
            // Put mines on the board
            MineCell cell;
            for (int i = 0; i < totalMine; ++i) {
                int found = -1;
                do
                {
                    idx = random.Next(i, m_lstCell.Length - 1);
                    found = System.Array.FindIndex<int>(m_lstMine, va => va == idx);

                } while (found != -1);

                m_lstMine[i] = idx;

                // set mine
                cell = m_lstCell[idx];
                cell.Info.isMine = true;
            }

            foreach (var obj in m_lstCell)
                if(!obj.Info.isMine)
                    obj.Info.mineCount = CalcNumMineBy(obj.m_idx);

            return true;
        }

		// Use this for initialization
		void Start()
        {
            Restart();
        }

        // index convert into X & Y location value
        void IndexToXY(int idx,out int x, out int y)
        {
            x = 0;
            y = 0;
            if(boardSize.First > 0) {
                x = idx % boardSize.First;
                y = idx / boardSize.First;
            }
        }

        // Location value convert into Index number
        int XYtoIdx(int x, int y) 
        {
            if (x < 0 || x >= boardSize.First)
                return -1;
            if (y < 0 || y >= boardSize.Second)
                return -1;
            return y * boardSize.First + x;
        }

        // Grab CellInfos surrounded by index
        List<CellInfo> GetCellsByIdx(int idx) 
        {
            int x, y;
            //CellInfo[] lst = new CellInfo[8];
            List<CellInfo> lst = new List<CellInfo>();

            IndexToXY(idx, out x, out y);
            for (int i = y - 1; i <= y + 1;++i)
                for (int j = x - 1; j <= x + 1;++j) {
                    int num = XYtoIdx(j,i);
                    if (num != idx && num > -1)
                        lst.Add(m_lstCell[num].Info);
                }
            
            return lst;
        }

        // Mine Number
        int CalcNumMineBy(int idx) {

            int count = 0;
            var lst = GetCellsByIdx(idx);
            foreach( var obj in lst)
                if (obj.isMine)
                    count++;
            return count;
        }

        // Invoke function
        public bool Invoke(BoardCmd cmd,int idx) 
        {
            MineCell cell = this.GetCell(idx);
            if (null == cell)
                return false;
            
            switch(cmd) {
                case BoardCmd.Confirm:
                    if(!cell.Info.Open()) {
                        this.GameOver();
                    }
                    break;
                case BoardCmd.FlagOn:
                    cell.Info.isFlaged = true;
                    break;
                case BoardCmd.FlagOff:
                    cell.Info.isFlaged = false;
                    break;
            }
            cell.Refresh();
            return true;
        }

        MineCell GetCell(int idx) {
            if(idx > -1 && idx < m_lstCell.Length)
                return m_lstCell[idx];
            return null;
        }
    }

}
