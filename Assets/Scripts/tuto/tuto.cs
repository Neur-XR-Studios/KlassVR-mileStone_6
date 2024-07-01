using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class tuto : MonoBehaviour
{
    // Start is called before the first frame update
    public class MyCollection : IEnumerable<int>
    {
        //IEnumerable<T> is an interface in C# that represents a sequence of elements of type T that can be enumerated (iterated over).
        //GetEnumerator() method returns an instance of a class that implements the IEnumerator<T> interface.
        private List<int> data; // List to store integers

        public MyCollection()
        {
            data = new List<int>();
        }

        public void Add(int item)
        {
            data.Add(item);
        }

        // Implementing GetEnumerator method from IEnumerable<int>
        //returning an instance of a class that implements the IEnumerator<T>
        public IEnumerator<int> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        // Implementing GetEnumerator method from IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    partial class MyClass
    {
        public void Method1() 
        {
            Debug.Log("hi");
        }
    }
    partial class MyClass
    {
        public void Method2()
        {
            Debug.Log("hi");
        }
    }
    public sealed class SealedClass
    {
        // We cannot Inherit 
    }

    delegate void MyDeligate(int item);
    MyDeligate deligateRef; 
    void Start()
    {
        //string reverse
        ReverseString();
        ReffAndOut();
        Enumarator();
        Deligate();
        StringBuild();
        PartialClass();
    }
    public void PartialClass()
    {
        //Partial classes help organize large classes by allowing developers to split them into smaller, more manageable parts.
        MyClass myClass = new MyClass();
        myClass.Method1();
        myClass.Method2();
    }
    public void StringBuild()
    {
        String str = "Hello";
        str += ", World!"; // Concatenating strings creates a new string
       Debug.Log(str); //immutable

        StringBuilder stringBuilder = new StringBuilder("Hello");
        stringBuilder.Append(", World!"); // Modifying StringBuilder directly
       Debug.Log(stringBuilder);
        //StringBuilder is particularly useful when you need to perform a lot of string manipulation operations,
        //such as concatenation or modification, as it can be more efficient than using regular string concatenation.

    }
    public void Using()
    {
      //The using statement ensures that the Dispose method is called on an Instance, typically
     //used for releasing unmanaged resources such as file handles or database connections.
        using (var fileStream = new FileStream("example.txt", FileMode.Open))
        {
            // Use fileStream here
        } // fileStream.Dispose() is called automatically here

        // Using statement for aliasing
        File.WriteAllText("example.txt", "Hello, world!"); // No need to fully qualify
    }
    
    public void Deligate()
    {
        deligateRef = PrintNmber;
        //This line invokes the delegate ,The delegate, when invoked, will execute the method that it points to
        deligateRef(50);


    }
    public void PrintNmber(int i)
    {
        Debug.Log(i);
    }
    public void ReffAndOut()
    {
        int a = 5;
        int b;
        //  the variable passed as an argument must be initialized before it's passed to the method.
        //can read and modify the value of the variable.
        ModifyWithRef(ref a);
        // doesn't need to be initialized before it's passed to the method.
        //must be assigned a value before the method returns
        InitializeWithOut(out b);

        ModifyWithRef(ref a);
        Console.WriteLine(a);

        InitializeWithOut(out b);
        Console.WriteLine(b);
    }

    async void DoAsyncWork()
    {
        //we asynchronously call SomeAsyncOperation() using await.
        //This means that the program won't block here and can continue executing other tasks while waiting for SomeAsyncOperation() to complete.
        Debug.Log("Starting the async operation.");
        // Simulate a time-consuming operation asynchronously
        await Task.Delay(3000);
    }
        public void Enumarator()
    {
        //IEnumerable represents a collection that can be enumerated
        //IEnumerator provides the ability to iterate over a collection.
        // it is a interface 
        // sequence of elements that can be iterated,does not represent a collection itself,
        // but rather provides a way to iterate over the elements of a collection.
        //indicating that instances of that class can be treated as collections of integers that can be iterated 

        //IMP ** The GetEnumerator() method is defined by the IEnumerable<int> interface.
        //this method returns an instance of a class that implements the IEnumerator<int> interface

        int[] numbers = { 1, 2, 3, 4, 5 };

        MyCollection collection = new MyCollection();

        // Add some elements to the collection
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);

        // Use a foreach loop to iterate over the collection
        Debug.Log("Using foreach loop:");
        foreach (int item in collection)
        {
            Debug.Log(item);
        }

        // Use LINQ to query the collection
        var query = collection.Where(x => x % 2 == 0);
        Debug.Log("\nUsing LINQ query:");
        foreach (int item in query)
        {
            Debug.Log(item);
        }
        //provides a way to iterate through the elements of a collection sequentially.
        IEnumerator enumerator = numbers.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int num = (int)enumerator.Current;
            Debug.Log(num);
        }
    }
    public void ReverseString()
    {
        string name = "ajay";
        string reverse = ReverseStr(name);
        Debug.Log(reverse);
    }
    string ReverseStr(string str)
    {
        char[] chars = str.ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }
    // Update is called once per frame
    void ModifyWithRef(ref int x)
    {
        x += 10;
    }
    void InitializeWithOut(out int y)
    {
        // y must be assigned a value before returning
        y = 20;
    }

}
