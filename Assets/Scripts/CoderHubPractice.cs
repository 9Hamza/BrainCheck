using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoderHubPractice : MonoBehaviour
{
    private int amount = 4;
    private int[] coins = new[] { 1, 2 };
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(coins_combinations(amount, coins));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static int coins_combinations(int amount, int[] coins) {
            // write your code here
            int combinations = 0;

            // loop over coins []
            for(int i = 0; i < coins.Length; i++) {
                if (self_combination(coins[i],amount) == true) {
                    combinations++;
                }
            }

            return combinations;
            // check coins[i] if it can be a combination itself
            // check if it can be one of i and one of i+1
        }

        private static bool self_combination(int number, int amount) {
            int sum = 0;
            while(number < amount) {
                sum += number;
                if(number == amount) {
                    return true;
                }
            }
            return false;
        }
}
