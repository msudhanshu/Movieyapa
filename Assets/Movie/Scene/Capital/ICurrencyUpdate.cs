
using System;
using System.Collections.Generic;


public interface ICurrencyUpdate
{
	ICapitalCurrency GetCurrency();
	int GetQuantity();
}


public interface ICurrencyCost <T> where T: ICurrencyUpdate
{
    List<T> GetCost();
    Dictionary<ICapitalCurrency,int> GetCostMap();
}

public interface ICurrencyReward<T> where T: ICurrencyUpdate
{
    List<T> GetReward();
    Dictionary<ICapitalCurrency,int> GetRewardMap();
}