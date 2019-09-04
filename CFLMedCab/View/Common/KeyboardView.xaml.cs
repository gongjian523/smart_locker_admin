using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CFLMedCab.View.Common
{
    /// <summary>
    /// KeyboardView.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class KeyboardView : Window
    {
        public static RoutedUICommand CmdTlide = new RoutedUICommand();
        public static RoutedUICommand Cmd1 = new RoutedUICommand();
        public static RoutedUICommand Cmd2 = new RoutedUICommand();
        public static RoutedUICommand Cmd3 = new RoutedUICommand();
        public static RoutedUICommand Cmd4 = new RoutedUICommand();
        public static RoutedUICommand Cmd5 = new RoutedUICommand();
        public static RoutedUICommand Cmd6 = new RoutedUICommand();
        public static RoutedUICommand Cmd7 = new RoutedUICommand();
        public static RoutedUICommand Cmd8 = new RoutedUICommand();
        public static RoutedUICommand Cmd9 = new RoutedUICommand();
        public static RoutedUICommand Cmd0 = new RoutedUICommand();
        public static RoutedUICommand CmdMinus = new RoutedUICommand();
        public static RoutedUICommand CmdPlus = new RoutedUICommand();
        public static RoutedUICommand CmdBackspace = new RoutedUICommand();

        public static RoutedUICommand CmdTab = new RoutedUICommand();
        public static RoutedUICommand CmdQ = new RoutedUICommand();
        public static RoutedUICommand Cmdw = new RoutedUICommand();
        public static RoutedUICommand CmdE = new RoutedUICommand();
        public static RoutedUICommand CmdR = new RoutedUICommand();
        public static RoutedUICommand CmdT = new RoutedUICommand();
        public static RoutedUICommand CmdY = new RoutedUICommand();
        public static RoutedUICommand CmdU = new RoutedUICommand();
        public static RoutedUICommand CmdI = new RoutedUICommand();
        public static RoutedUICommand CmdO = new RoutedUICommand();
        public static RoutedUICommand CmdP = new RoutedUICommand();
        public static RoutedUICommand CmdOpenCrulyBrace = new RoutedUICommand();
        public static RoutedUICommand CmdEndCrultBrace = new RoutedUICommand();
        public static RoutedUICommand CmdOR = new RoutedUICommand();

        public static RoutedUICommand CmdCapsLock = new RoutedUICommand();
        public static RoutedUICommand CmdA = new RoutedUICommand();
        public static RoutedUICommand CmdS = new RoutedUICommand();
        public static RoutedUICommand CmdD = new RoutedUICommand();
        public static RoutedUICommand CmdF = new RoutedUICommand();
        public static RoutedUICommand CmdG = new RoutedUICommand();
        public static RoutedUICommand CmdH = new RoutedUICommand();
        public static RoutedUICommand CmdJ = new RoutedUICommand();
        public static RoutedUICommand CmdK = new RoutedUICommand();
        public static RoutedUICommand CmdL = new RoutedUICommand();
        public static RoutedUICommand CmdColon = new RoutedUICommand();
        public static RoutedUICommand CmdDoubleInvertedComma = new RoutedUICommand();
        public static RoutedUICommand CmdEnter = new RoutedUICommand();

        public static RoutedUICommand CmdShift = new RoutedUICommand();
        public static RoutedUICommand CmdZ = new RoutedUICommand();
        public static RoutedUICommand CmdX = new RoutedUICommand();
        public static RoutedUICommand CmdC = new RoutedUICommand();
        public static RoutedUICommand CmdV = new RoutedUICommand();
        public static RoutedUICommand CmdB = new RoutedUICommand();
        public static RoutedUICommand CmdN = new RoutedUICommand();
        public static RoutedUICommand CmdM = new RoutedUICommand();
        public static RoutedUICommand CmdGreaterThan = new RoutedUICommand();
        public static RoutedUICommand CmdLessThan = new RoutedUICommand();
        public static RoutedUICommand CmdQuestion = new RoutedUICommand();

        public static RoutedUICommand CmdSpaceBar = new RoutedUICommand();
        public static RoutedUICommand CmdClear = new RoutedUICommand();

        private bool _ShiftFlag;
        protected bool ShiftFlag
        {
            get { return _ShiftFlag; }
            set { _ShiftFlag = value; }
        }

        private bool _CapsLockFlag;
        protected bool CapsLockFlag
        {
            get { return _CapsLockFlag; }
            set { _CapsLockFlag = value; }
        }

        private static Control _CurrentControl;
        public static string TouchScreenText
        {
            get
            {
                if (_CurrentControl is TextBox)
                {
                    return ((TextBox)_CurrentControl).Text;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    return ((PasswordBox)_CurrentControl).Password;
                }
                else return "";


            }
            set
            {
                if (_CurrentControl is TextBox)
                {
                    ((TextBox)_CurrentControl).Text = value;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    ((PasswordBox)_CurrentControl).Password = value;
                }
            }
        }

        public KeyboardView()
        {
            InitializeComponent();

            SetCommandBinding();
        }

        public void
            SetCurrentControl(Control currentControl)
        {
            _CurrentControl = currentControl;
        }

        private void SetCommandBinding()
        {
            CommandBinding CbTlide = new CommandBinding(CmdTlide, RunCommand);
            CommandBinding Cb1 = new CommandBinding(Cmd1, RunCommand);
            CommandBinding Cb2 = new CommandBinding(Cmd2, RunCommand);
            CommandBinding Cb3 = new CommandBinding(Cmd3, RunCommand);
            CommandBinding Cb4 = new CommandBinding(Cmd4, RunCommand);
            CommandBinding Cb5 = new CommandBinding(Cmd5, RunCommand);
            CommandBinding Cb6 = new CommandBinding(Cmd6, RunCommand);
            CommandBinding Cb7 = new CommandBinding(Cmd7, RunCommand);
            CommandBinding Cb8 = new CommandBinding(Cmd8, RunCommand);
            CommandBinding Cb9 = new CommandBinding(Cmd9, RunCommand);
            CommandBinding Cb0 = new CommandBinding(Cmd0, RunCommand);
            CommandBinding CbMinus = new CommandBinding(CmdMinus, RunCommand);
            CommandBinding CbPlus = new CommandBinding(CmdPlus, RunCommand);
            CommandBinding CbBackspace = new CommandBinding(CmdBackspace, RunCommand);

            CommandBinding CbTab = new CommandBinding(CmdTab, RunCommand);
            CommandBinding CbQ = new CommandBinding(CmdQ, RunCommand);
            CommandBinding Cbw = new CommandBinding(Cmdw, RunCommand);
            CommandBinding CbE = new CommandBinding(CmdE, RunCommand);
            CommandBinding CbR = new CommandBinding(CmdR, RunCommand);
            CommandBinding CbT = new CommandBinding(CmdT, RunCommand);
            CommandBinding CbY = new CommandBinding(CmdY, RunCommand);
            CommandBinding CbU = new CommandBinding(CmdU, RunCommand);
            CommandBinding CbI = new CommandBinding(CmdI, RunCommand);
            CommandBinding Cbo = new CommandBinding(CmdO, RunCommand);
            CommandBinding CbP = new CommandBinding(CmdP, RunCommand);
            CommandBinding CbOpenCrulyBrace = new CommandBinding(CmdOpenCrulyBrace, RunCommand);
            CommandBinding CbEndCrultBrace = new CommandBinding(CmdEndCrultBrace, RunCommand);
            CommandBinding CbOR = new CommandBinding(CmdOR, RunCommand);

            CommandBinding CbCapsLock = new CommandBinding(CmdCapsLock, RunCommand);
            CommandBinding CbA = new CommandBinding(CmdA, RunCommand);
            CommandBinding CbS = new CommandBinding(CmdS, RunCommand);
            CommandBinding CbD = new CommandBinding(CmdD, RunCommand);
            CommandBinding CbF = new CommandBinding(CmdF, RunCommand);
            CommandBinding CbG = new CommandBinding(CmdG, RunCommand);
            CommandBinding CbH = new CommandBinding(CmdH, RunCommand);
            CommandBinding CbJ = new CommandBinding(CmdJ, RunCommand);
            CommandBinding CbK = new CommandBinding(CmdK, RunCommand);
            CommandBinding CbL = new CommandBinding(CmdL, RunCommand);
            CommandBinding CbColon = new CommandBinding(CmdColon, RunCommand);
            CommandBinding CbDoubleInvertedComma = new CommandBinding(CmdDoubleInvertedComma, RunCommand);
            CommandBinding CbEnter = new CommandBinding(CmdEnter, RunCommand);

            CommandBinding CbShift = new CommandBinding(CmdShift, RunCommand);
            CommandBinding CbZ = new CommandBinding(CmdZ, RunCommand);
            CommandBinding CbX = new CommandBinding(CmdX, RunCommand);
            CommandBinding CbC = new CommandBinding(CmdC, RunCommand);
            CommandBinding CbV = new CommandBinding(CmdV, RunCommand);
            CommandBinding CbB = new CommandBinding(CmdB, RunCommand);
            CommandBinding CbN = new CommandBinding(CmdN, RunCommand);
            CommandBinding CbM = new CommandBinding(CmdM, RunCommand);
            CommandBinding CbGreaterThan = new CommandBinding(CmdGreaterThan, RunCommand);
            CommandBinding CbLessThan = new CommandBinding(CmdLessThan, RunCommand);
            CommandBinding CbQuestion = new CommandBinding(CmdQuestion, RunCommand);

            CommandBinding CbSpaceBar = new CommandBinding(CmdSpaceBar, RunCommand);
            CommandBinding CbClear = new CommandBinding(CmdClear, RunCommand);

            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbTlide);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb1);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb2);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb3);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb4);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb5);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb6);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb7);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb8);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb9);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cb0);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbMinus);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbPlus);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbBackspace);

            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbTab);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbQ);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cbw);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbE);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbR);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbT);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbY);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbU);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbI);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), Cbo);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbP);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbOpenCrulyBrace);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbEndCrultBrace);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbOR);

            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbCapsLock);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbA);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbS);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbD);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbF);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbG);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbH);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbJ);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbK);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbL);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbColon);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbDoubleInvertedComma);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbEnter);

            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbShift);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbZ);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbX);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbC);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbV);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbB);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbN);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbM);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbGreaterThan);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbLessThan);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbQuestion);

            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbSpaceBar);
            CommandManager.RegisterClassCommandBinding(typeof(KeyboardView), CbClear);

        }

        void RunCommand(object sender, ExecutedRoutedEventArgs e)
        {

            if (e.Command == CmdTlide)  //First Row
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "`";
                }
                else
                {
                    TouchScreenText += "~";
                    ShiftFlag = false;
                }
            }
            else if (e.Command == Cmd1)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "1";
                }
                else
                {
                    TouchScreenText += "!";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd2)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "2";
                }
                else
                {
                    TouchScreenText += "@";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd3)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "3";
                }
                else
                {
                    TouchScreenText += "#";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd4)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "4";
                }
                else
                {
                    TouchScreenText += "$";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd5)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "5";
                }
                else
                {
                    TouchScreenText += "%";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd6)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "6";
                }
                else
                {
                    TouchScreenText += "^";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd7)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "7";
                }
                else
                {
                    TouchScreenText += "&";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd8)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "8";
                }
                else
                {
                    TouchScreenText += "*";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd9)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "9";
                }
                else
                {
                    TouchScreenText += "(";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == Cmd0)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "0";
                }
                else
                {
                    TouchScreenText += ")";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdMinus)
            {
                if (!ShiftFlag)
                {
                    KeyboardView.TouchScreenText += "-";
                }
                else
                {
                    TouchScreenText += "_";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdPlus)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "=";
                }
                else
                {
                    TouchScreenText += "+";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdBackspace)
            {
                if (!string.IsNullOrEmpty(TouchScreenText))
                {
                    TouchScreenText = TouchScreenText.Substring(0, TouchScreenText.Length - 1);
                }

            }
            else if (e.Command == CmdTab)  //Second Row
            {
                TouchScreenText += "     ";
            }
            else if (e.Command == CmdQ)
            {
                AddKeyBoardINput('Q');
            }
            else if (e.Command == Cmdw)
            {
                AddKeyBoardINput('w');
            }
            else if (e.Command == CmdE)
            {
                AddKeyBoardINput('E');
            }
            else if (e.Command == CmdR)
            {
                AddKeyBoardINput('R');
            }
            else if (e.Command == CmdT)
            {
                AddKeyBoardINput('T');
            }
            else if (e.Command == CmdY)
            {
                AddKeyBoardINput('Y');
            }
            else if (e.Command == CmdU)
            {
                AddKeyBoardINput('U');

            }
            else if (e.Command == CmdI)
            {
                AddKeyBoardINput('I');
            }
            else if (e.Command == CmdO)
            {
                AddKeyBoardINput('O');
            }
            else if (e.Command == CmdP)
            {
                AddKeyBoardINput('P');
            }
            else if (e.Command == CmdOpenCrulyBrace)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "[";
                }
                else
                {
                    TouchScreenText += "{";
                    ShiftFlag = false;
                }
            }
            else if (e.Command == CmdEndCrultBrace)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "]";
                }
                else
                {
                    TouchScreenText += "}";
                    ShiftFlag = false;
                }
            }
            else if (e.Command == CmdOR)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += @"\";
                }
                else
                {
                    TouchScreenText += "|";
                    ShiftFlag = false;
                }
            }
            else if (e.Command == CmdCapsLock)  ///Third ROw
            {

                if (!CapsLockFlag)
                {
                    CapsLockFlag = true;
                }
                else
                {
                    CapsLockFlag = false;

                }
            }
            else if (e.Command == CmdA)
            {
                AddKeyBoardINput('A');
            }
            else if (e.Command == CmdS)
            {
                AddKeyBoardINput('S');
            }
            else if (e.Command == CmdD)
            {
                AddKeyBoardINput('D');
            }
            else if (e.Command == CmdF)
            {
                AddKeyBoardINput('F');
            }
            else if (e.Command == CmdG)
            {
                AddKeyBoardINput('G');
            }
            else if (e.Command == CmdH)
            {
                AddKeyBoardINput('H');
            }
            else if (e.Command == CmdJ)
            {
                AddKeyBoardINput('J');
            }
            else if (e.Command == CmdK)
            {
                AddKeyBoardINput('K');
            }
            else if (e.Command == CmdL)
            {
                AddKeyBoardINput('L');

            }
            else if (e.Command == CmdColon)
            {
                if (!ShiftFlag)
                {
                    KeyboardView.TouchScreenText += ";";
                }
                else
                {
                    TouchScreenText += ":";
                    ShiftFlag = false;
                }
            }
            else if (e.Command == CmdDoubleInvertedComma)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "'";
                }
                else
                {
                    TouchScreenText += Char.ConvertFromUtf32(34);
                    ShiftFlag = false;
                }
            }
            else if (e.Command == CmdEnter)
            {
                Hide();            
                //_CurrentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Command == CmdShift) //Fourth Row
            {

                ShiftFlag = true; ;


            }
            else if (e.Command == CmdZ)
            {
                AddKeyBoardINput('Z');

            }
            else if (e.Command == CmdX)
            {
                AddKeyBoardINput('X');

            }
            else if (e.Command == CmdC)
            {
                AddKeyBoardINput('C');

            }
            else if (e.Command == CmdV)
            {
                AddKeyBoardINput('V');

            }
            else if (e.Command == CmdB)
            {
                AddKeyBoardINput('B');

            }
            else if (e.Command == CmdN)
            {
                AddKeyBoardINput('N');

            }
            else if (e.Command == CmdM)
            {
                AddKeyBoardINput('M');

            }
            else if (e.Command == CmdLessThan)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += ",";
                }
                else
                {
                    TouchScreenText += "<";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdGreaterThan)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += ".";
                }
                else
                {
                    TouchScreenText += ">";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdQuestion)
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += "/";
                }
                else
                {
                    TouchScreenText += "?";
                    ShiftFlag = false;
                }

            }
            else if (e.Command == CmdSpaceBar)//Last row
            {

                TouchScreenText += " ";
            }
            else if (e.Command == CmdClear)//Last row
            {

                TouchScreenText = "";
            }
        }


        private void AddKeyBoardINput(char input)
        {
            if (CapsLockFlag)
            {
                if (ShiftFlag)
                {
                    TouchScreenText += char.ToLower(input).ToString();
                    ShiftFlag = false;

                }
                else
                {
                    TouchScreenText += char.ToUpper(input).ToString();
                }
            }
            else
            {
                if (!ShiftFlag)
                {
                    TouchScreenText += char.ToLower(input).ToString();
                }
                else
                {
                    TouchScreenText += char.ToUpper(input).ToString();
                    ShiftFlag = false;
                }
            }
        }
    }
    
}
