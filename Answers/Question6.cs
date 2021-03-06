﻿using System.Runtime.CompilerServices;

namespace C_Sharp_Challenge_Skeleton.Answers
{
    public class Question6
    {
        /* when in doubt, roll your own priority queue */
        struct State
        {
            public int p, n;
            public State(int _p, int _n)
            {
                p = _p;
                n = _n;
            }
        }
        static unsafe class PQ
        {
            static State* s;
            public static int edx;
            static int* ixs;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Init(State* _s, int* _ixs)
            {
                s = _s;
                ixs = _ixs;
                edx = 1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void r_up(int idx)
            {
                int p = idx / 2;
                while (idx != 1)
                {
                    if (s[idx].p > s[p].p) return;
                    /* rotate up */
                    (s[idx], s[p]) = (s[p], s[idx]);
                    (ixs[s[idx].n], ixs[s[p].n]) = (idx, p);
                    idx = p;
                    p = idx / 2;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void r_down(int idx)
            {
                int nx = idx * 2;
                while (nx < edx)
                {
                    /* rotate down */
                    if (edx != nx + 1 && s[nx + 1].p < s[nx].p) nx++;
                    if (s[nx].p > s[idx].p) return;
                    (s[nx], s[idx]) = (s[idx], s[nx]);
                    (ixs[s[idx].n], ixs[s[nx].n]) = (idx, nx);
                    idx = nx;
                    nx = idx * 2;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Push(int n, int p)
            {
                s[edx] = new State(p, n);
                ixs[n] = edx;
                r_up(edx);
                edx++;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Update(int n, int p)
            {
                int idx = ixs[n];
                int cp = s[idx].p;
                if (cp == p) return;
                s[idx].p = p;
                if (cp > p)
                {
                    /* priority reduction, balance up */
                    r_up(idx);
                }
                else
                {
                    /* priority increase, balance down */
                    r_down(idx);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int Pop(out int p)
            {
                State ret = s[1];
                s[1] = s[edx - 1];
                edx--;
                r_down(1);
                p = ret.p;
                ixs[ret.n] = 0;
                return ret.n;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool Contains(int n)
            {
                return (ixs[n] != 0);
            }
        }

        public static unsafe int Answer(int numOfServers, int targetServer, int[,] connectionTimeMatrix)
        {
            int sz = numOfServers + 10;
            State* s = stackalloc State[sz];
            int* ixs = stackalloc int[sz];
            PQ.Init(s, ixs);
            bool* v = stackalloc bool[sz];
            int* sp = stackalloc int[sz];
            sp[0] = 0;
            for (int i = 1; i < numOfServers; i++) sp[i] = 1<<30;
            PQ.Push(0, 0);
            int cn, p;
            while (PQ.edx != 1)
            {
                cn = PQ.Pop(out p);
                if (cn == targetServer) return p;
                if (v[cn]) continue;
                v[cn] = true;
                for (int i = 1; i < numOfServers; i++)
                {
                    if (v[i]) continue;
                    int np = sp[cn] + connectionTimeMatrix[cn, i];
                    if (np < sp[i])
                    {
                        if (PQ.Contains(i)) PQ.Update(i, np);
                        else PQ.Push(i, np);
                        sp[i] = np;
                    }
                }
            }
            return connectionTimeMatrix[0, targetServer];
        }
    }
}