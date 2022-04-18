using System.Collections;
using System.Collections.Generic;
using System;


public static class CardListUtil
{
    private static Random _ran = new Random();

    public static void Shuffle<T>(this List<T> list)
    {
        int to = list.Count;
        while(to > 1)
        {
            int from = _ran.Next(--to + 1);
            T tmp = list[from];
            list[from] = list[to];
            list[to] = tmp;
        }
    }

    public static List<Card[]> GenerateFiveCombi(List<Card> cards)
    {
        /* Get Five combi */
        List<int[]> combiPos = generateCombinations(cards.Count, 5); // index + 1
        List<Card[]> allCombinationsOfCard = new List<Card[]>(); // Holds all the combinations

        foreach(int[] place in combiPos)
        {
            Card[] temp = new Card[5];
			temp[0] = cards[place[0] - 1];
			temp[1] = cards[place[1] - 1];
			temp[2] = cards[place[2] - 1];
			temp[3] = cards[place[3] - 1];
			temp[4] = cards[place[4] - 1];
			allCombinationsOfCard.Add(temp);           
        }
        return allCombinationsOfCard;
    }


    public static void sortCards(this List<Card> cards)
    {
        cards.Sort(
            (x, y) =>
            x == null ? (y == null ? 0 : -1) : (y == null ? 1 : x.num.CompareTo(y.num))
            );
        
        cards.Reverse();
    }




    private static List<int[]> generateCombinations(int n, int r) 
    {
        List<int[]> combinations = new List<int[]>();
        generateCombinationsRecursively(combinations, new int[r], 1, n, 0);
        return combinations;
    }

    private static void generateCombinationsRecursively(List<int[]> combinations, int[] elements, int current, int end, int index) 
    {
        if (index == elements.Length) {
            int[] temp = elements.Clone() as int[];
            combinations.Add(temp);
            return;
        }
        if (current > end) {
            return;
        }
        elements[index] = current;
        generateCombinationsRecursively(combinations, elements, current + 1, end, index + 1);
        generateCombinationsRecursively(combinations, elements, current + 1, end, index);
    }

}
