namespace Classes;

public class BankAccount
{
    private static int accountNumberSeed = 1234567890;
    private readonly decimal _minimumBalance;
    public string Number { get; }
    public string Owner { get; set; }
    public decimal Balance
    {
        get
        {
            decimal balance=0;
            foreach(var item in allTransactions)
            {
                balance+=item.Amount;
            }
            return balance;
        }
    }
    
    private List<Transaction> allTransactions = new List<Transaction>();
    
    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0) { }

    public BankAccount(string name, decimal initialBalance,decimal minimumBalance)
    {
        Owner = name;
        Number = accountNumberSeed.ToString();
        _minimumBalance=minimumBalance;
        accountNumberSeed++;
        if(initialBalance>0)
            MakeDeposit(initialBalance,DateTime.Now,"Initial Balance");
    }
    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if(amount<=0){
            throw new ArgumentOutOfRangeException(nameof(amount),"Amount of deposit must be positive");
        }
        var deposit=new Transaction(amount,date,note);
        allTransactions.Add(deposit);
    }
    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of withdrawal must be positive");
        }   
        Transaction? overdraftTransaction = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);
        allTransactions.Add(withdrawal);
        if (overdraftTransaction != null)
            allTransactions.Add(overdraftTransaction);
    }

    protected virtual Transaction? CheckWithdrawalLimit(bool isOverdrawn)
    {
        if (isOverdrawn)
        {
            throw new InvalidOperationException("Not sufficient funds for this withdrawal");
        }
        else
        {
            return default;
        }
    }

    public string GetAccountHistory(){
        string TransactionHistory=$"{this.Owner}---{this.Number}---Transaction History\n";
        foreach(var item in allTransactions){
            if(item.Amount<=0){
                TransactionHistory+=$"Withdraw {-1*item.Amount} on {item.Date} for {item.Notes} \n";
            }
            else{
                 TransactionHistory+=$"Deposit {item.Amount} on {item.Date} for {item.Notes} \n";
            }
        }
        TransactionHistory+=$"Current Balance is {this.Balance}";
        return TransactionHistory;
    }
    public virtual void PerformMonthEndTransactions(){}
}