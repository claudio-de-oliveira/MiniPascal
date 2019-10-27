using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    public class Block : KeyedData
    {
        private int id;

        public int row;
        public int col;

        public int dfs_entry;
        public int dfs_exit;

        public List<Block> childs;
        public bool Visited;
        public int dfn;

        private int leader;
        private AbsMachine code;
        private ArrayOfBlock sucessor;
        private ArrayOfBlock predecessor;

        private string key;

        public Block(int leader)
        {
            key = leader.ToString();

            id = -1;
        
            row = 0;
            col = 0;

            dfs_entry = -1;
            dfs_exit = -1;
            Visited = false;
            dfn = -1;
            childs = new List<Block>();

            this.leader = leader;
            code = new AbsMachine();
            sucessor = new ArrayOfBlock();
            predecessor = new ArrayOfBlock();
        }

        public override string Key
        { get { return key; } }

        public AbsMachine Code
        { get { return code; } }

        public int Id
        { 
            get { return id; }
            set { id = value; }
        }

        public IntermediateInstruction Last
        {
            get { 
                if (code.Count > 0)
                    return code[code.Count - 1]; 
                else
                    return new Nop();
            }
        }

        public int Size
        { get { return code.Count; } }

        public IntermediateInstruction this[int i]
        { get { Debug.Assert(i >= 0 && i < Size); return code[i]; } }

        public void Add(IntermediateInstruction inst)
        {
            code.AddInstruction(inst);
        }

        public void Insert(int pos, IntermediateInstruction inst)
        {
            code.InsertInstruction(pos, inst);
        }

        public ArrayOfBlock Predecessor
        { 
            get { return predecessor; }
            set { predecessor = value; } 
        }
        public ArrayOfBlock Sucessor
        { 
            get { return sucessor; }
            set { sucessor = value; }
        }

        public int Leader
        { get { return leader; } }

        public override string ToString()
        {
            return id.ToString();
        }
    }

    public class GraphOfBlock : LGraph
    {
        public GraphOfBlock(string key)
            : base(key)
        { 
            // Nothing more todo
        }

        public bool Split(int blk, int pos)
        {
            if (pos > 0)
            {
                Block block = (Block)this[blk].Vertex;
                Block newBlk1 = new Block(block.Leader);
                Block newBlk2 = new Block(block.Leader + pos);

                for (int i = 0; i < block.Size; i++)
                {
                    if (i < pos)
                        newBlk1.Add(block[i]);
                    else
                        newBlk2.Add(block[i]);
                }

                LNode n1 = new LNode(newBlk1);
                LNode n2 = new LNode(newBlk2);

                foreach (LNode pred in this[blk].Predecessor)
                {
                    if (pred != this[blk])
                    {
                        n1.Predecessor.Add(pred);
                        if (((Block)pred.Vertex).Last.GetType() != typeof(Nop))
                        {
                            switch (((Block)pred.Vertex).Last[0].Op)
                            {
                                case Operator.CALL:
                                case Operator.GOTO:
                                case Operator.IFEXP:
                                case Operator.IFTRUE:
                                case Operator.IFFALSE:
                                    if (((Label)(((Block)pred.Vertex).Last[0].Arg2)).Blk == this[blk].Vertex)
                                        ((Block)pred.Vertex).Last[0].Arg2 = new Label(newBlk1);
                                    break;
                                default:
                                    break;
                            }
                        }
                        pred.Successor.Remove(this[blk]);
                        pred.Successor.Add(n1);
                    }
                    else
                    {
                        n1.Predecessor.Add(n2);
                        n2.Successor.Add(n1);
                    }
                }
                foreach (LNode succ in this[blk].Successor)
                {
                    if (succ != this[blk])
                    {
                        n2.Successor.Add(succ);
                        succ.Predecessor.Remove(this[blk]);
                        succ.Predecessor.Add(n2);
                    }
                    else
                    {
                        n2.Successor.Add(n1);
                        n1.Predecessor.Add(n2);
                    }

                }
                n1.Successor.Add(n2);
                n2.Predecessor.Add(n1);

                this[blk].Successor.RemoveAll((LNode) => true);
                this[blk].Predecessor.RemoveAll((LNode) => true);

                nodes.Remove(this[blk]);

                nodes.Insert(blk, n1);
                nodes.Insert(blk + 1, n2);

                return true;
            }
            return false;
        }

        public void Split()
        {
            for (int i = 1; i < Count - 1; i++)
            {
                for (int j = 1; j < ((Block)this[i].Vertex).Size; j++)
                {
                    if (((Block)this[i].Vertex)[j].GetType() != typeof(Nop))
                    {
                        switch (((Block)this[i].Vertex)[j][0].Op)
                        {
                            case Operator.ADD:
                            case Operator.SUB:
                            case Operator.DIV:
                            case Operator.MUL:
                                Split(i, j);
                                break;
                        }
                    }
                }
            }

            for (int k = 0; k < Count; k++)
                ((Block)this[k].Vertex).Id = k;
        }

        public void Split1()
        {
            int nBlocks = Count;

            for (int i = 1; i < Count; i++)
            {
                LNode node = (LNode)this[i];

                if (node.Predecessor.Count > 1)
                {
                    for (int p = 0; p < node.Predecessor.Count; p++)
                    {
                        LNode pred = node.Predecessor[p];

                        LNode newNode = new LNode(new Block(((Block)node.Vertex).Leader));
                        ((Block)newNode.Vertex).Add(new Nop());
                        newNode.Predecessor.Add(new LNode(pred.Vertex));
                        newNode.Successor.Add(node);
                        pred.Successor.Remove(node);
                        pred.Successor.Add(newNode);
                        node.Predecessor[p] = newNode;
                        if (((Block)this[i].Vertex).Last.GetType() != typeof(Nop))
                        {
                            switch (((Block)pred.Vertex).Last[0].Op)
                            {
                                case Operator.CALL:
                                case Operator.GOTO:
                                case Operator.IFEXP:
                                case Operator.IFTRUE:
                                case Operator.IFFALSE:
                                    if (((Label)((Block)pred.Vertex).Last[0].Arg2).Blk == node.Vertex)
                                        ((Block)pred.Vertex).Last[0].Arg2 = new Label((Block)newNode.Vertex);
                                    break;
                                default:
                                    break;
                            }
                        }
                        nodes.Insert(Count - 1, newNode);
                    }
                }
            }

            for (int k = 0; k < Count; k++)
                ((Block)this[k].Vertex).Id = k;
        }


        public BitSet[] Dominators()
        {
            BitSet[] IN = new BitSet[this.Count];
            BitSet[] OUT = new BitSet[this.Count];
            HashSet<object> universe = new HashSet<object>();

            foreach (LNode u in this)
                universe.Add(u.Vertex);

            OUT[0] = new BitSet(universe);
            IN[0] = new BitSet(universe);
            OUT[0].SetToZeros();
            OUT[0][0] = true;
            for (int bk = 1; bk < Count; bk++)
            {
                OUT[bk] = new BitSet(universe);
                IN[bk] = new BitSet(universe);
                OUT[bk].SetToOnes();
            }

            bool done = false;

            while (!done)
            {
                done = true;

                for (int bk = 1; bk < Count; bk++)
                {
                    IN[bk].SetToOnes();
//                    foreach (Block pred in this[bk].Predecessor)
//                        IN[bk] = IN[bk] * OUT[pred.Id];
                    string str1 = IN[bk].ToBinary();

                    BitSet old = OUT[bk];
                    for (int i = 0; i < universe.Count; i++)
                        OUT[bk][i] = IN[bk][i];
                    OUT[bk][bk] = true;
                    done = done && (OUT[bk] == old);
                }
            }

            return OUT;
        }

    }

    public class ArrayOfBlock : List<Block>
    {
        public enum BlockState
        {
            White, Gray, Black
        }

        public void Split()
        {
            for (int i = 1; i < Count - 1; i++)
            {
                for (int j = 1; j < this[i].Size; j++)
                {
                    if (this[i][j].GetType() != typeof(Nop))
                    {
                        switch (this[i][j][0].Op)
                        {
                            case Operator.ADD:
                            case Operator.SUB:
                            case Operator.DIV:
                            case Operator.MUL:
                                Split(i, j);
                                break;
                        }
                    }
                }
            }

            for (int k = 0; k < Count; k++)
                this[k].Id = k;
        }

        public bool Split(int blk, int pos)
        {
            if (pos > 0)
            {
                Block newBlk1 = new Block(this[blk].Leader);
                Block newBlk2 = new Block(this[blk].Leader + pos);

                for (int i = 0; i < this[blk].Size; i++)
                    if (i < pos)
                        newBlk1.Add(this[blk][i]);
                    else
                        newBlk2.Add(this[blk][i]);

                foreach (Block pred in this[blk].Predecessor)
                {
                    if (pred != this[blk])
                    {
                        newBlk1.Predecessor.Add(pred);
                        if (pred.Last.GetType() != typeof(Nop))
                        {
                            switch (pred.Last[0].Op)
                            {
                                case Operator.CALL:
                                case Operator.GOTO:
                                case Operator.IFEXP:
                                case Operator.IFTRUE:
                                case Operator.IFFALSE:
                                    if (((Label)pred.Last[0].Arg2).Blk == this[blk])
                                        pred.Last[0].Arg2 = new Label(newBlk1);
                                    break;
                                default:
                                    break;
                            }
                        }
                        pred.Sucessor.Remove(this[blk]);
                        pred.Sucessor.Add(newBlk1);
                    }
                    else
                    {
                        newBlk1.Predecessor.Add(newBlk2);
                        newBlk2.Sucessor.Add(newBlk1);
                    }
                }
                foreach (Block succ in this[blk].Sucessor)
                {
                    if (succ != this[blk])
                    {
                        newBlk2.Sucessor.Add(succ);
                        succ.Predecessor.Remove(this[blk]);
                        succ.Predecessor.Add(newBlk2);
                    }
                    else
                    {
                        newBlk2.Sucessor.Add(newBlk1);
                        newBlk1.Predecessor.Add(newBlk2);
                    }
                    
                }
                newBlk1.Sucessor.Add(newBlk2);
                newBlk2.Predecessor.Add(newBlk1);

                Predicate<Block> match = new Predicate<Block>((Block) => true);
                this[blk].Sucessor.RemoveAll(match);
                this[blk].Predecessor.RemoveAll(match);
                Remove(this[blk]);

                Insert(blk, newBlk1);
                Insert(blk + 1, newBlk2);

                return true;
            }
            return false;
        }

        public void Split1()
        {
            int nBlocks = Count;

            for (int i = 1; i < nBlocks; i++)
            {
                Block blk = this[i];

                if (blk.Predecessor.Count > 1)
                {
                    for (int p = 0; p < blk.Predecessor.Count; p++)
                    {
                        Block pred = blk.Predecessor[p];

                        Block newBlock = new Block(blk.Leader);
                        newBlock.Add(new Nop());
                        newBlock.Predecessor.Add(pred);
                        newBlock.Sucessor.Add(blk);
                        pred.Sucessor.Remove(blk);
                        pred.Sucessor.Add(newBlock);
                        blk.Predecessor[p] = newBlock;
                        if (blk.Last.GetType() != typeof(Nop))
                        {
                            switch (pred.Last[0].Op)
                            {
                                case Operator.CALL:
                                case Operator.GOTO:
                                case Operator.IFEXP:
                                case Operator.IFTRUE:
                                case Operator.IFFALSE:
                                    if (((Label)pred.Last[0].Arg2).Blk == blk)
                                        pred.Last[0].Arg2 = new Label(newBlock);
                                    break;
                                default:
                                    break;
                            }
                        }
                        Insert(Count - 1, newBlock);
                    }
                }
            }

            for (int k = 0; k < Count; k++)
                this[k].Id = k;
        }

        public BitSet[] Dominators()
        {
            BitSet[] IN = new BitSet[this.Count];
            BitSet[] OUT = new BitSet[this.Count];
            HashSet<object> universe = new HashSet<object>();

//            foreach (object obj in elements)
//                universe.Add(obj);

            OUT[0] = new BitSet(universe);
            IN[0] = new BitSet(universe);
            OUT[0].SetToZeros();
            OUT[0][0] = true;
            for (int bk = 1; bk < Count; bk++)
            {
                OUT[bk] = new BitSet(universe);
                IN[bk] = new BitSet(universe);
                OUT[bk].SetToOnes();
            }

            bool done = false;

            while (!done)
            {
                done = true;

                for (int bk = 1; bk < Count; bk++)
                {
                    IN[bk].SetToOnes();
                    foreach (Block pred in this[bk].Predecessor)
                        IN[bk] = IN[bk] * OUT[pred.Id];
                    string str1 = IN[bk].ToBinary();

                    BitSet old = OUT[bk];
                    for (int i = 0; i < universe.Count; i++)
                        OUT[bk][i] = IN[bk][i];
                    OUT[bk][bk] = true;
                    done = done && (OUT[bk] == old);
                }
            }

            return OUT;
        }

//        private void Search(Block blk, ref int c)
//        {
//            blk.Visited = true;
//            foreach (Block succ in blk.Sucessor)
//            {
//                if (succ.Visited == false)
//                {
//                    blk.childs.Add(succ);
//                    Search(succ, ref c);
//                }
//            }
//            blk.dfn = c;
//            c--;
//        }
//
//        public void Search()
//        {
//            int c = Count;
//            Search(this[0], ref c); 
//        }

        #region Rotinas para impressão
        private void runDFS(Block u, BlockState[] state, Queue<object> Q)
        {
            state[u.Id] = BlockState.Gray;
            for (int i = 0; i < u.Sucessor.Count; i++)
            {
                Block v = u.Sucessor[i];

                if (state[v.Id] == BlockState.White)
                {
                    v.row = u.row + 1;
                    if (u.Sucessor.Count > 1)
                    {
                        if (i == 0)
                            v.col = u.col - 1;
                        else
                            v.col = u.col + i;
                    }
                    else
                        v.col = u.col;

                    runDFS(v, state, Q);
                }
            }
            state[u.Id] = BlockState.Black;
            Q.Enqueue(u);
        }

        public void DFS(Queue<object> Q)
        {
            BlockState[] state = new BlockState[Count];
            for (int i = 0; i < Count; i++)
                state[i] = BlockState.White;
            this[0].row = 0;
            this[0].col = 0;
            runDFS(this[0], state, Q);
        }

        public override string ToString()
        {
            string str = "";

            if (Count > 0)
            {
                int i;

                str += "\\{";
                for (i = 0; i < Count - 1; i++)
                    str += this[i].Id.ToString() + ", ";
                str += this[i].Id.ToString() + "\\}";
            }

            return str;
        }
        #endregion

        public string ToLaTeX()
        {
            string str = "\\begin{scriptsize}\n\\xy";
            int max = int.MinValue;
            int min = int.MaxValue;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].col < min)
                    min = this[i].col;
                if (this[i].col > max)
                    max = this[i].col;
            }

            for (int i = 0; i < Count; i++)
            {
                str += "(" + (25 * this[i].col).ToString() + ", " + (-15 * this[i].row) + ")\n\t*++{\\txt{";
                str += "$\\mathbf{B_{" + i.ToString() + "}}$\\\\";
                for (int j = 0; j < this[i].Code.Count; j++)
                    if (this[i][j].GetType() != typeof(Nop))
                        str += "$" + this[i][j].ToString() + "$\\\\";
                str += "}}*\\frm{-,}=\"B" + i.ToString() + "\";\n";
            }
            for (int i = 0; i < Count; i++)
                foreach (Block succ in this[i].Sucessor)
                    str += "\"B" + i.ToString() + "\";\"B" + succ.Id.ToString() + "\" **@{.} ?> *{\\dir{>}};\n";
            str += "\\endxy\n\\end{scriptsize}\n";

            return str;
        }
    }
}
