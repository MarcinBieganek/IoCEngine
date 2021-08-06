using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zadanie_2
{
    interface IFoo
    {
        int Test();
    }

    class Foo : IFoo
    {
        public int Test()
        {
            return 2;
        }
    }

    class Bar : IFoo
    {
        public int Test()
        {
            return 3;
        }
    }

    abstract class TAbs { }

    class A
    {
        public B b;

        public A(B b)
        {
            this.b = b;
        }
    }

    class B { }

    class X
    {
        public Y d;
        public string s;
        public X(Y d, string s) 
        { 
            this.d = d;
            this.s = s;
        }
    }

    class Y { }

    class Te
    {
        public string A;
        public string B;
        
        [DependencyConstructor]
        public Te()
        {
            A = "te const 1";
            B = "te const 1";
        }

        public Te(string a)
        {
            A = a;
            B = "te const 2";
        }
        public Te(string a, string b)
        {
            A = a;
            B = b;
        }
    }

    class Te2
    {
        public string A;
        public string B;
        
        [DependencyConstructor]
        public Te2()
        {
            A = "te const 1";
            B = "te const 1";
        }
        [DependencyConstructor]
        public Te2(string a)
        {
            A = a;
            B = "te const 2";
        }
        public Te2(string a, string b)
        {
            A = a;
            B = b;
        }
    }

    class Te3
    {
        public string A;
        public string B;
        
        public Te3()
        {
            A = "te const 1";
            B = "te const 1";
        }
        public Te3(string a)
        {
            A = a;
            B = "te const 2";
        }
        public Te3(string a, string b)
        {
            A = a;
            B = b;
        }
    }

    class AA {
        public BB b;
        private CC _c;
        public DD d;
        public EE e;
        private FF _f;

        public AA(BB b) 
        {
            this.b = b;
        }

        [DependencyProperty]
        public CC TheC 
        { 
            get { return this._c; }
            set { this._c = value; } 
        }

        [DependencyMethod]
        public void setD(DD d) 
        {
            this.d = d;
        }

        public void setE(EE e)
        {
            this.e = e;
        }

        public FF TheF 
        { 
            get { return this._f; }
            set { this._f = value; } 
        }
    }

    class BB { }
    class CC { }
    class DD { }
    class EE { }
    class FF { }

    class Astring 
    {
        public string a;

        [DependencyMethod]
        public void setA(string ab) 
        {
            this.a = ab;
        }
    }

    class Astring2 
    {
        private string _a;

        [DependencyProperty]
        public string TheA 
        { 
            get { return this._a; }
            set { this._a = value; } 
        }
    }

    class ACycle
    {
        private ACycle _ac;

        [DependencyProperty]
        public ACycle TheA 
        { 
            get { return this._ac; }
            set { this._ac = value; } 
        }
    }
    class ACycle2
    {
        private ACycle2 _ac;

        [DependencyMethod]
        public void SetAC(ACycle2 ac)
        { 
            this._ac = ac;
        }
    }

    class Cycle
    {
        public Cycle(Cycle c) {}
    }

    class ACycle_BuildUp
    {
        public Cycle c;

        [DependencyProperty]
        public Cycle TheC
        { 
            get { return this.c; }
            set { this.c = value; } 
        }
    }

    class ACycle_BuildUp2
    {
        public Cycle c;

        [DependencyMethod]
        public void SetC(Cycle c)
        { 
            this.c = c;
        }
    }

    [TestClass]
    public class UnitTestListaBZadanie2
    {
        [TestMethod]
        public void TestBuildUpInjectionThruProperty()
        {
            SimpleContainer c = new SimpleContainer();
            AA theA = new AA(new BB());

            Assert.IsNotNull(theA.b);
            Assert.IsNull(theA.TheC);

            c.BuildUp<AA>(theA);

            Assert.IsNotNull(theA.b);
            Assert.IsNotNull(theA.TheC);
        }

        [TestMethod]
        public void TestBuildUpInjectionThruMethod()
        {
            SimpleContainer c = new SimpleContainer();
            AA theA = new AA(new BB());

            Assert.IsNotNull(theA.b);
            Assert.IsNull(theA.d);

            c.BuildUp<AA>(theA);

            Assert.IsNotNull(theA.b);
            Assert.IsNotNull(theA.d);
        }

        [TestMethod]
        public void TestBuildUpNoneInjectionThruMethodWithoutAttribute()
        {
            SimpleContainer c = new SimpleContainer();
            AA theA = new AA(new BB());
            c.BuildUp<AA>(theA);

            Assert.IsNull(theA.e);
        }

        [TestMethod]
        public void TestBuildUpNoneInjectionThruPropertyWithoutAttribute()
        {
            SimpleContainer c = new SimpleContainer();
            AA theA = new AA(new BB());
            c.BuildUp<AA>(theA);

            Assert.IsNull(theA.TheF);
        }

        [TestMethod]
        public void TestBuildUpInjectionThruMethodWithUnregisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            Astring astr = new Astring();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    c.BuildUp<Astring>(astr);
                }
            );
        }

        [TestMethod]
        public void TestBuildUpInjectionThruMethodWithRegisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("abc");

            Astring astr = new Astring();
            c.BuildUp<Astring>(astr);

            Assert.AreEqual(astr.a, "abc");
        }

        [TestMethod]
        public void TestBuildUpInjectionThruPropertyWithUnregisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            Astring2 astr2 = new Astring2();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    c.BuildUp<Astring2>(astr2);
                }
            );
        }

        [TestMethod]
        public void TestBuildUpInjectionThruPropertyWithRegisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("abc");

            Astring2 astr2 = new Astring2();
            c.BuildUp<Astring2>(astr2);

            Assert.AreEqual(astr2.TheA, "abc");
        }

        [TestMethod]
        public void TestBuildUpCycleInProperty()
        {
            SimpleContainer c = new SimpleContainer();
            ACycle_BuildUp acb = new ACycle_BuildUp();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    c.BuildUp<ACycle_BuildUp>(acb);
                }
            );
        }

        [TestMethod]
        public void TestBuildUpCycleInMethod()
        {
            SimpleContainer c = new SimpleContainer();
            ACycle_BuildUp2 acb = new ACycle_BuildUp2();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    c.BuildUp<ACycle_BuildUp2>(acb);
                }
            );
        }
    }

    [TestClass]
    public class UnitTestListaBZadanie1
    {
        [TestMethod]
        public void TestInjectionThruProperty()
        {
            SimpleContainer c = new SimpleContainer();
            AA a = c.Resolve<AA>();

            Assert.IsNotNull(a.TheC);
        }

        [TestMethod]
        public void TestInjectionThruMethod()
        {
            SimpleContainer c = new SimpleContainer();
            AA a = c.Resolve<AA>();

            Assert.IsNotNull(a.d);
        }

        [TestMethod]
        public void TestNoneInjectionThruMethodWithoutAttribute()
        {
            SimpleContainer c = new SimpleContainer();
            AA a = c.Resolve<AA>();

            Assert.IsNull(a.e);
        }

        [TestMethod]
        public void TestNoneInjectionThruPropertyWithoutAttribute()
        {
            SimpleContainer c = new SimpleContainer();
            AA a = c.Resolve<AA>();

            Assert.IsNull(a.TheF);
        }

        [TestMethod]
        public void TestInjectionThruMethodWithUnregisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    Astring astr = c.Resolve<Astring>();
                }
            );
        }

        [TestMethod]
        public void TestInjectionThruMethodWithRegisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("abc");

            Astring astr = c.Resolve<Astring>();

            Assert.AreEqual(astr.a, "abc");
        }

        [TestMethod]
        public void TestInjectionThruPropertyWithUnregisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    Astring2 astr2 = c.Resolve<Astring2>();
                }
            );
        }

        [TestMethod]
        public void TestInjectionThruPropertyWithRegisteredInstance()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("abc");

            Astring2 astr2 = c.Resolve<Astring2>();

            Assert.AreEqual(astr2.TheA, "abc");
        }

        [TestMethod]
        public void TestCycleInProperty()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    ACycle ac = c.Resolve<ACycle>();
                }
            );
        }

        [TestMethod]
        public void TestCycleInMethod()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    ACycle2 ac2 = c.Resolve<ACycle2>();
                }
            );
        }
    }

    [TestClass]
    public class UnitTestListaAZadanie2
    {
        [TestMethod]
        public void TestSimpleDependencyInjection()
        {
            SimpleContainer c = new SimpleContainer();
            A a = c.Resolve<A>();

            Assert.IsNotNull(a.b);
        }

        [TestMethod]
        public void TestResolveFailNoneNoParameterConstructor()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    X x = c.Resolve<X>();
                }
            );
        }

        [TestMethod]
        public void TestResolveAfterRegisterInstance()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterInstance<string>("ala ma kota");
            X x = c.Resolve<X>();

            Assert.IsNotNull(x.d);
            Assert.AreEqual(x.s, "ala ma kota");
        }

        [TestMethod]
        public void TestOneDependencyConstrutors()
        {
            SimpleContainer c = new SimpleContainer();

            Te t = c.Resolve<Te>();

            Assert.AreEqual(t.A, "te const 1");
            Assert.AreEqual(t.B, "te const 1");
        }

        [TestMethod]
        public void TestTwoDependencyConstrutors()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("ala ma kota");

            Te2 t = c.Resolve<Te2>();

            Assert.AreEqual(t.A, "ala ma kota");
            Assert.AreEqual(t.B, "ala ma kota");
        }

        [TestMethod]
        public void TestZeroDependencyConstrutors()
        {
            SimpleContainer c = new SimpleContainer();
            c.RegisterInstance<string>("ala ma kota");

            Te3 t = c.Resolve<Te3>();

            Assert.AreEqual(t.A, "ala ma kota");
            Assert.AreEqual(t.B, "ala ma kota");
        }
    }

    [TestClass]
    public class UnitTestListaAZadanie1
    {
        [TestMethod]
        public void TestSimpleRegisterInstance()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();
            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);
        }

        [TestMethod]
        public void TestRegisterInstanceTwiceTheSame()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();
            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);
            Assert.AreSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterInstanceTwiceDifferent()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();
            IFoo foo2 = new Foo();

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);

            c.RegisterInstance<IFoo>( foo2 );
            
            IFoo foo4 = c.Resolve<IFoo>();

            Assert.AreSame(foo2, foo4);
            Assert.AreNotSame(foo3, foo4);
        }

        [TestMethod]
        public void TestRegisterInstanceAfterRegisterType()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterType<Foo>( false );

            IFoo foo2 = c.Resolve<Foo>();

            Assert.AreNotSame(foo1, foo2);

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterInstanceAfterRegisterTypeSingleton()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterType<Foo>( true );

            IFoo foo2 = c.Resolve<Foo>();

            Assert.AreNotSame(foo1, foo2);

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterTypeAfterRegisterInstance()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);

            c.RegisterType<Foo>( false );
            
            IFoo foo3 = c.Resolve<Foo>();

            Assert.AreNotSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterTypeAfterRegisterInstanceSingleton()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);

            c.RegisterType<Foo>( true );
            
            IFoo foo3 = c.Resolve<Foo>();

            Assert.AreNotSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterInstanceAfterRegisterImplementation()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterType<IFoo, Foo>( false );

            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreNotSame(foo1, foo2);

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterInstanceAfterRegisterImplementationSingleton()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterType<IFoo, Foo>( true );

            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreNotSame(foo1, foo2);

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterImplementationAfterRegisterInstance()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreNotSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }

        [TestMethod]
        public void TestRegisterImplementationAfterRegisterInstanceSingleton()
        {
            SimpleContainer c = new SimpleContainer();

            IFoo foo1 = new Foo();

            c.RegisterInstance<IFoo>( foo1 );
            
            IFoo foo2 = c.Resolve<IFoo>();

            Assert.AreSame(foo1, foo2);

            c.RegisterType<IFoo, Foo>( true );
            
            IFoo foo3 = c.Resolve<IFoo>();

            Assert.AreNotSame(foo1, foo3);
            Assert.AreNotSame(foo2, foo3);
        }
    }

    [TestClass]
    public class UnitTestLista9Zadanie1
    {
        [TestMethod]
        public void TestSimpleResolveType()
        {
            SimpleContainer c = new SimpleContainer();
            
            Foo fs = c.Resolve<Foo>();

            Assert.IsInstanceOfType(fs, typeof(Foo));
        }

        [TestMethod]
        public void TestSimpleResolveWorking()
        {
            SimpleContainer c = new SimpleContainer();
            
            Foo fs = c.Resolve<Foo>();

            int t = fs.Test();

            Assert.AreEqual(t, 2);
        }

        [TestMethod]
        public void TestSimpleResolveNotEqual()
        {
            SimpleContainer c = new SimpleContainer();
            
            Foo f1 = c.Resolve<Foo>();
            Foo f2 = c.Resolve<Foo>();

            Assert.AreNotSame(f1, f2);
        }

        [TestMethod]
        public void TestRegisterSingletonEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<Foo>( true );
            
            Foo f1 = c.Resolve<Foo>();
            Foo f2 = c.Resolve<Foo>();

            Assert.AreSame(f1, f2);
        }

        [TestMethod]
        public void TestRegisterSingletonNotEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<Foo>( false );
            
            Foo f1 = c.Resolve<Foo>();
            Foo f2 = c.Resolve<Foo>();

            Assert.AreNotSame(f1, f2);
        }

        [TestMethod]
        public void TestRegisterTwoSingleton()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<Foo>( true );
            c.RegisterType<Bar>( false );
            
            Foo f1 = c.Resolve<Foo>();
            Foo f2 = c.Resolve<Foo>();

            Assert.AreSame(f1, f2);

            Bar b1 = c.Resolve<Bar>();
            Bar b2 = c.Resolve<Bar>();

            Assert.AreNotSame(b1, b2);
        }

        [TestMethod]
        public void TestRegisterImplementation()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));
        }

        [TestMethod]
        public void TestRegisterImplementationChange()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));

            c.RegisterType<IFoo, Bar>( false );

            IFoo f2 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f2, typeof(Bar));
        }

        [TestMethod]
        public void TestRegisterImplementationWorking()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));

            int f1Test = f1.Test();

            Assert.AreEqual(f1Test, 2);
        }

        [TestMethod]
        public void TestRegisterImplementationChangeWorking()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));

            c.RegisterType<IFoo, Bar>( false );

            IFoo f2 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f2, typeof(Bar));

            int f2Test = f2.Test();

            Assert.AreEqual(f2Test, 3);
        }

        [TestMethod]
        public void TestRegisterImplementationSingletonEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( true );
            
            IFoo f1 = c.Resolve<IFoo>();
            IFoo f2 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));
            Assert.IsInstanceOfType(f2, typeof(Foo));

            Assert.AreSame(f1, f2);
        }

        [TestMethod]
        public void TestRegisterImplementationSingletonNotEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();
            IFoo f2 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));
            Assert.IsInstanceOfType(f2, typeof(Foo));

            Assert.AreNotSame(f1, f2);
        }

        [TestMethod]
        public void TestRegisterImplementationChangeSingletonEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));

            c.RegisterType<IFoo, Bar>( true );

            IFoo f2 = c.Resolve<IFoo>();
            IFoo f3 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f2, typeof(Bar));
            Assert.IsInstanceOfType(f3, typeof(Bar));

            Assert.AreSame(f2, f3);
        }

        [TestMethod]
        public void TestRegisterImplementationChangeSingletonNotEqual()
        {
            SimpleContainer c = new SimpleContainer();

            c.RegisterType<IFoo, Foo>( false );
            
            IFoo f1 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f1, typeof(Foo));

            c.RegisterType<IFoo, Bar>( false );

            IFoo f2 = c.Resolve<IFoo>();
            IFoo f3 = c.Resolve<IFoo>();

            Assert.IsInstanceOfType(f2, typeof(Bar));
            Assert.IsInstanceOfType(f3, typeof(Bar));

            Assert.AreNotSame(f2, f3);
        }

        [TestMethod]
        public void TestResolveInterface()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    IFoo f1 = c.Resolve<IFoo>();
                }
            );
        }

        [TestMethod]
        public void TestResolveAbstractClass()
        {
            SimpleContainer c = new SimpleContainer();

            Assert.ThrowsException<ArgumentException>(
                () => {
                    TAbs t = c.Resolve<TAbs>();
                }
            );
        }

    }
}
