using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memory_Policy_Simulator
{
    class Core
    {
        private int cursor;
        public int p_frame_size;
        public Queue<Page> frame_window;
        public List<Page> frame_window_list;
        public List<Page> pageHistory;
        public List<char> stack = new List<char>();             //for LRU
        public List<bool> referenceBit = new List<bool>();      //for 'second Chance' & 'additional referance Bit'
        public List<int> counter = new List<int>();             //for MFU & LFU
        public int hit;
        public int fault;
        public int migration;
        public int hand=0;

        public Core(int get_frame_size)
        {
            this.cursor = 0;
            this.p_frame_size = get_frame_size;
            this.frame_window_list = new List<Page>();
            this.frame_window = new Queue<Page>();
            this.pageHistory = new List<Page>();
        }

        public Page.STATUS OperateMFU(char data)
        {
            Page newPage;

            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                cursor = i + 1;
                newPage.loc = cursor;
                counter[cursor - 1]++;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;

                    int MaxCount = counter.ElementAt(0);
                    int MaxIndex = 0;
                    for (int i = 0; i < frame_window_list.Count; i++)
                    {
                        if (MaxCount < counter.ElementAt(i)) { MaxCount = counter.ElementAt(i); MaxIndex = i; }
                    }
                    cursor = MaxIndex + 1;
                    counter.RemoveAt(cursor - 1);
                    counter.Add(0);
                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    counter.Add(0);
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
            }
            pageHistory.Add(newPage);
            return newPage.status;
        }

        public Page.STATUS OperateLFU(char data)
        {
            Page newPage;

            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                cursor = i + 1;
                newPage.loc = cursor;
                counter[cursor - 1]++;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;

                    int MinCount = counter.ElementAt(0);
                    int MinIndex = 0;
                    for (int i = 0; i < frame_window_list.Count; i++)
                    {
                        if (MinCount > counter.ElementAt(i)) { MinCount = counter.ElementAt(i); MinIndex = i; }
                    }
                    cursor=MinIndex + 1;
                    counter.RemoveAt(cursor - 1);
                    counter.Add(0);
                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    counter.Add(0);
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
            }
            pageHistory.Add(newPage);
            return newPage.status;
        }



        public Page.STATUS OperateSecondChance(char data)
        {
            Page newPage;

            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                cursor = i + 1;
                newPage.loc = cursor;
                referenceBit[cursor - 1] = true;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;
                    bool find = false;

                    for (; hand < frame_window_list.Count; hand++)
                    {
                        if (referenceBit.ElementAt(hand) == false)
                        {
                            find = true; cursor = hand + 1; break;
                        }
                        referenceBit[hand] = false;
                    }
                    if (find == false)
                    {
                        for(hand=0; hand < frame_window_list.Count; hand++)
                        {
                            if (referenceBit.ElementAt(hand) == false)
                            {
                                cursor = hand + 1; break;
                            }
                            referenceBit[hand] = false;
                        }
                    }
                    referenceBit[cursor - 1] = true;

                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Insert(cursor - 1, newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    referenceBit.Add(false);
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
            }
            pageHistory.Add(newPage);
            return newPage.status;
        }

        public Page.STATUS OperateAdditionalReferenceBits(char data)
        {
            Page newPage;

            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                cursor = i + 1;
                newPage.loc = cursor;
                referenceBit[cursor - 1] = true;
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;
                    cursor = referenceBit.FindIndex(x => x == false);
                    if (cursor == -1) cursor = 1;
                    else cursor += 1;

                    referenceBit.RemoveAt(cursor - 1);
                    referenceBit.Add(false);

                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    referenceBit.Add(false);
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
            }
            pageHistory.Add(newPage);
            return newPage.status;
        }


        public Page.STATUS OperateLRU(char data)
        {
            Page newPage;
            
            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;
                stack.Remove(data);
                stack.Add(data);
            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;
                    char change = stack.ElementAt(0);
                    cursor = this.frame_window_list.FindIndex(x => x.data == change)+1;

                    stack.Add(data);
                    stack.RemoveAt(0);

                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Insert(cursor - 1, newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    stack.Add(data);
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }
            }
            pageHistory.Add(newPage);
            return newPage.status;
        }



        public Page.STATUS OperateOptimal(char data, String datas, int index)
        {
            Page newPage;

            if (this.frame_window_list.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window_list.Count; i++)
                {
                    if (this.frame_window_list.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window_list.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;

                    List<int> list = new List<int>(); 
                    for(int i=0; i < this.frame_window_list.Count; i++)
                    {
                        String coming = datas.Substring(index);
                        char c = this.frame_window_list.ElementAt(i).data;
                        list.Add(coming.IndexOf(c));
                    }

                    int max = -1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list.ElementAt(i) == -1) {max=-1; cursor =i+1; break; }
                        if (max < list.ElementAt(i)) { max = list.ElementAt(i); cursor = i+1; }
                    }
                    this.frame_window_list.RemoveAt(cursor - 1);
                    this.migration++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Insert(cursor-1,newPage);
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    cursor++;
                    this.fault++;
                    newPage.loc = cursor;
                    frame_window_list.Add(newPage);
                }

            }
            pageHistory.Add(newPage);
            return newPage.status;
        }



        public Page.STATUS OperateFIFO(char data)
        {
            Page newPage;

            if (this.frame_window.Any<Page>(x => x.data == data))
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;
                newPage.status = Page.STATUS.HIT;
                this.hit++;
                int i;

                for (i = 0; i < this.frame_window.Count; i++)
                {
                    if (this.frame_window.ElementAt(i).data == data) break;
                }
                newPage.loc = i + 1;

            }
            else
            {
                newPage.pid = Page.CREATE_ID++;
                newPage.data = data;

                if (frame_window.Count >= p_frame_size)
                {
                    newPage.status = Page.STATUS.MIGRATION;
                    this.frame_window.Dequeue();
                    cursor = p_frame_size;
                    this.migration++;
                    this.fault++;
                }
                else
                {
                    newPage.status = Page.STATUS.PAGEFAULT;
                    cursor++;
                    this.fault++;
                }

                newPage.loc = cursor;
                frame_window.Enqueue(newPage);
            }
            pageHistory.Add(newPage);

            return newPage.status;
        }




        public List<Page> GetPageInfo(Page.STATUS status)
        {
            List<Page> pages = new List<Page>();

            foreach (Page page in pageHistory)
            {
                if (page.status == status)
                {
                    pages.Add(page);
                }
            }

            return pages;
        }
        

    }


}