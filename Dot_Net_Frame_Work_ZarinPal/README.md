# Dot Net Framework, ZarinPal Pay Action Reference
A simple reference to do payments on zarinapl in a dot net framework project / program.

# Features
- Fast Payment Action Callback
- Secured And Safe Payments
- Clean & Less Coding
- Auto Check Expire After 30 Minutes
- Easy To Use 

# Make your app support payments by 14 lines of sciprt!

```c#
zarinpal.pay payment = new zarinpal.pay("MerchantId", Amount, "Description", "CallBackUrl(can be google even)", "Email(Optional)", "Mobile(Optional)");
string auth = payment.StartPay(); // now got authority code!
System.Diagnostics.Process.Start(payment.URL + auth); // OPEN BROWSER OF USER TO DO THE PAYMENT :D
payment.OnPaymentAction += (s, e) => // Get Payment Status
{
    if (e.RefID == 100) // PAYMENT WAS SUCESSFULL xD, God, I've got more few dollars :)
    {
        MessageBox.Show("Thank you bro, love you xD");
    }
    else // The mofo user didn't pay the cash haha xD
    {
        MessageBox.Show("Why bro :( me need money please :)");
    }
};
```
