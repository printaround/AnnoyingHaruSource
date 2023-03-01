namespace SimpleGNovel
{
    public partial class MainWindow : Window
    {
        #region Base Values
        public static MainWindow _window; //Объект, используемый в методе перемещения главного окна

        private int maxTextsCount = 0;           //Кол-во строк во всём получаемом тексте (сценарии)
        private string imgName = "";             //Название фоновой картинки
        private string CharSpriteName = "";      //Название спрайта
        private int textID = 0;                  //Номер текущей строки текста
        private string[] textsArr;               //Весь получаемый текст (сценарий)
        private string story = "";               //Весь предыдущий текст (история)
        private string nameOfGamer = "";         //Имя игрока, которое будет отображаться в тексте
        private float animationTime = 1f;        //Скорость (время выполнения) анимаций персонажей


        private DispatcherTimer dispatcherTimer;            //Объект, используемый для анимации вывода текста (построчно)
        private string buildString = "";                    //Строка, посимвольно собирающая исходную строку, для анимации вывода текста (собирающая строка)
        private int indexOfBuildString = 0;                 //Номер текущего символа собирающей строки
        private int textSpeed = 9;                          //Скорость анимации текста (вывода строки)
        #endregion

        #region Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainWindow()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();

            _window = this;                                              //инициализация объекта для перемещения окна (с помощью мышки)

            _window.WindowState = WindowState.Maximized;                 //Запуск программы в полноэкранном режиме

            textsArr = (ReadFromFile("texts/text_1.txt")).Split('$');   //Получение всего текста (сценария) и разделение на строки (разделяющий спец.символ '$')

            try
            {
                for (int i = 0; i < textsArr.Length; i++)                   //Вырезать комментарии из текста (разделяющий спец.символ '&')
                {
                    if (textsArr[i].Contains('&'))
                        textsArr[i] = textsArr[i].Remove(textsArr[i].LastIndexOf('&') - 1);
                }
            }
            catch 
            { 
                MessageBox.Show("Похоже в сценарий были внесены изменения и теперь всё сломалось...", "Повреждённый текст!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            maxTextsCount = textsArr.Length;                            //Вычисление общего кол-ва строк

            SaveBtn.Visibility = Visibility.Collapsed;                  //Кнопка сохранения недоступна в меню
            moveBack.IsEnabled = false;                                 //Отображение истории будет доступно только после первого действия
            moveBack.Visibility = Visibility.Collapsed;
            mainText.Visibility = Visibility.Collapsed;
            moveNext.Visibility = Visibility.Collapsed;

            StringInRTB(Convert.ToString(ReadFromFile(@"DataSaveName.txt").Trim()), yourName);
        }
        private void SetNameConditions()
        {
            nameOfGamer = ReadFromFile(@"DataSaveName.txt").Trim();               //Получение указанного имени

            //Проверка корректности указанного имени и установка дефолтного в исключающих случаях

            if (nameOfGamer != "Акира")                                 //Заменить имя игрока в тексте, на указанное
            {
                for (int i = 0; i < textsArr.Length; i++)
                {
                    if (textsArr[i].Contains("Акира"))
                        textsArr[i] = textsArr[i].Replace("Акира", nameOfGamer);
                }
            }

            if (nameOfGamer == "DAR")                                  //Режим разработчика
                textSpeed = 0;
            else
                textSpeed = 9;

            //Создание объекта (таймера) для анимации текста + установка скорости анимации

            dispatcherTimer = new DispatcherTimer();
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, textSpeed);
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)   //Анимация вывода текста
        {
            buildString += textsArr[textID][indexOfBuildString];

            StringInRTB(buildString, mainText);
            indexOfBuildString++;

            if (indexOfBuildString >= textsArr[textID].Length)
            {
                indexOfBuildString = 0;
                buildString = "";
                dispatcherTimer.Stop();
            }

            CommandManager.InvalidateRequerySuggested();
        }
        #endregion

        #region Moving window with mouse
        private void Drug(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) { MainWindow._window.DragMove(); }
        }
        #endregion

        #region Main Plot
        private void PlotFlow()
        {
            if (indexOfBuildString != 0) return;    //Если достигнута последняя строка, то метод не выполнится

            if (textID == 0)
            {
                SetNameConditions();
                yourName.Visibility = Visibility.Collapsed;
                startBtn.Visibility = Visibility.Collapsed;
                menuExitBtn.Visibility = Visibility.Collapsed;
                warningName.Visibility = Visibility.Collapsed;
                confirmNameBtn.Visibility = Visibility.Collapsed;

                SaveBtn.Visibility = Visibility.Visible;
                moveBack.Visibility = Visibility.Visible;
                mainText.Visibility = Visibility.Visible;
                moveNext.Visibility = Visibility.Visible;

                LoadBtn.Margin = new Thickness(122, 10, 0, 0);
                LoadBtn.Width = 100;
                LoadBtn.Height = 36;
                LoadBtn.FontSize = 16;
                LoadBtn.Opacity = 0.80f;
            }

            if ((textID + 1) >= maxTextsCount) return;

            textID++;
            SetTextColor();

            switch (textID)
            {
                #region Сцена "Пробуждение"
                case 1: { imgName = "dark"; BackPlay("backMus1"); moveBack.IsEnabled = true; break; }
                case 20: { SoundPlay("door"); BackPlay("silent"); break; }
                case 21: { SoundPlay("onniichan"); break; }
                case 22: { BackPlay("backMus2"); CharSpriteName = "4_2_love"; Anim_ClosingIn(1000, 1100); break; }
                #endregion

                #region Сцена "Первый разговор в комнате"
                case 23: { imgName = "Bedroom_Day"; CharSpriteName = "2_standart"; Anim_ClosingIn(1100, 1000); break; }
                case 26: { SoundPlay("giggle"); CharSpriteName = "8_happy"; break; }
                case 29: { CharSpriteName = "7_sad"; break; }
                case 30: { CharSpriteName = "8_happy"; break; }
                case 33: { CharSpriteName = "4_love"; break; }
                case 34: { CharSpriteName = "3_shocked"; break; }
                case 35: { SoundPlay("rumbling"); break; }
                case 36: { CharSpriteName = "7_sad"; break; }
                case 38: { CharSpriteName = "2_standart"; break; }
                case 39: { Anim_GoOut(); break; }
                case 40: { BackPlay("backMus1"); CharSpriteName = "void"; Anim_StandUp(); break; }
                #endregion

                #region Сцена "На кухне"
                case 48: { CharSpriteName = "2_standart"; imgName = "Kitchen_Day"; BackPlay("backMus3"); break; }
                case 49: { CharSpriteName = "4_2_love"; break; }
                case 50: { CharSpriteName = "2_standart"; break; }
                case 53: { CharSpriteName = "3_shocked"; break; }
                case 54: { CharSpriteName = "7_sad"; break; }
                case 56: { CharSpriteName = "8_2_happy"; break; }
                case 57: { CharSpriteName = "8_happy"; break; }
                case 60: { CharSpriteName = "2_standart"; break; }
                case 62: { CharSpriteName = "8_2_happy"; break; }
                case 63: { CharSpriteName = "2_standart"; break; }
                case 67: { CharSpriteName = "4_love"; break; }
                case 69: { CharSpriteName = "3_shocked"; break; }
                case 78: { CharSpriteName = "4_2_love"; break; }
                case 80: { CharSpriteName = "7_sad"; break; }
                #endregion

                #region Сцена "Флешбек с прошлой ночи"
                case 84: { CharSpriteName = "void"; imgName = "dark"; BackPlay("silent"); break; }
                case 85: { imgName = "Bedroom_Night_Dark"; break; }
                case 86: { SoundPlay("ring"); break; }
                case 88: { imgName = "Small_Apartment_Kitchen_Night"; SoundPlay("ring"); break; }
                case 90: { imgName = "Apartment_Exterior_Night"; CharSpriteName = "11_2_snow"; BackPlay("sity_night"); break; }
                case 93: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Прогулка-серьёзный разговор"
                case 94: { imgName = "Street_Summer_Day"; CharSpriteName = "8_happy"; BackPlay("street_walk"); break; }
                case 96: { CharSpriteName = "2_standart"; break; }
                case 99: { CharSpriteName = "7_sad"; break; }
                case 104: { CharSpriteName = "5_angry"; break; }
                case 106: { CharSpriteName = "3_shocked"; break; }
                case 108: { CharSpriteName = "7_sad"; break; }
                case 112: { CharSpriteName = "4_love"; break; }
                case 114: { CharSpriteName = "2_standart"; break; }
                case 115: { CharSpriteName = "7_sad"; break; }
                case 118: { CharSpriteName = "8_happy"; break; }
                case 120: { CharSpriteName = "8_2_happy"; break; }
                case 122: { CharSpriteName = "7_sad"; break; }
                case 126: { CharSpriteName = "5_angry"; break; }
                case 128: { CharSpriteName = "7_sad"; break; }
                case 137: { CharSpriteName = "6_sleepy"; break; }
                case 138: { CharSpriteName = "2_standart"; break; }
                case 139: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Ванная"
                case 142: { imgName = "Bathroom_Foggy"; CharSpriteName = "8_2_happy"; BackPlay("bath"); break; }
                case 145: { CharSpriteName = "4_2_love"; break; }
                case 149: { imgName = "Small_Apartment_Kitchen"; CharSpriteName = "void"; break; }
                case 156: { imgName = "Bedroom_Day"; BackPlay("silent"); break; }
                case 158: { BackPlay("backMus1"); break; }
                case 178: { imgName = "dark"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Ресторан"
                case 180: { imgName = "Restaurant_B"; CharSpriteName = "2_standart"; BackPlay("backMus3"); break; }
                case 183: { CharSpriteName = "3_shocked"; break; }
                case 186: { CharSpriteName = "7_sad"; break; }
                case 191: { CharSpriteName = "4_love"; break; }
                case 193: { CharSpriteName = "4_2_love"; break; }
                case 195: { CharSpriteName = "8_happy"; break; }
                case 196: { CharSpriteName = "8_2_happy"; break; }
                case 205: { CharSpriteName = "8_happy"; break; }
                case 206: { SoundPlay("giggle"); break; }
                case 208: { CharSpriteName = "2_standart"; break; }
                case 210: { CharSpriteName = "6_sleepy"; break; }
                case 213: { CharSpriteName = "2_standart"; break; }
                case 215: { CharSpriteName = "4_love"; break; }
                case 218: { CharSpriteName = "3_shocked"; break; }
                case 219: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Вечерняя прогулка"
                case 221: { imgName = "Street_Summer_Night"; CharSpriteName = "11_2_snow"; BackPlay("sity_night"); break; }
                case 231: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Время косплея"
                case 233: { imgName = "Livingroom_Night"; CharSpriteName = "11_snow"; BackPlay("silent"); break; }
                case 236: { Anim_GoOut(); break; }
                case 241: { Anim_StandUp(); CharSpriteName = "10_2_wow"; SoundPlay("meow"); BackPlay("BackMus2"); break; }
                case 243: { CharSpriteName = "9_1_wow"; break; }
                case 249: { Anim_GoOut(); break; }
                case 254: { Anim_StandUp(); CharSpriteName = "8_2_happy"; break; }
                case 255: { CharSpriteName = "8_happy"; break; }
                case 258: { CharSpriteName = "7_sad"; break; }
                case 260: { CharSpriteName = "4_2_love"; break; }
                case 261: { CharSpriteName = "7_sad"; break; }
                case 267: { CharSpriteName = "6_sleepy"; break; }
                case 271: { CharSpriteName = "7_sad"; break; }
                case 272: { CharSpriteName = "8_happy"; break; }
                case 273: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Перед сном"
                case 277: { imgName = "Livingroom_Dark"; break; }
                case 280: { CharSpriteName = "6_2_sleepy"; break; }
                case 281: { Anim_GoOut(); break; }
                case 282: { imgName = "dark"; CharSpriteName = "void"; Anim_StandUp(); break; }
                #endregion

                #region Сцена "Утро без Хару"
                case 285: { imgName = "Livingroom_Day"; break; }
                case 287: { imgName = "Small_Apartment_Kitchen"; break; }
                case 288: { imgName = "Bathroom"; break; }
                case 289: { BackPlay("bath"); break; }
                case 290: { imgName = "Small_Apartment_Kitchen"; break; }
                case 291: { imgName = "Kitchen_Day"; BackPlay("cooking"); break; }
                case 299: { SoundPlay("door"); break; }
                case 301: { CharSpriteName = "7_sad"; break; }
                case 302: { BackPlay("silent"); break; }
                case 308: { imgName = "dark"; CharSpriteName = "void"; break; }
                #endregion

                #region Сцена "Разговор по душам"
                case 309: { imgName = "Bedroom_Day"; CharSpriteName = "7_sad"; break; }
                case 312: { BackPlay("backMus1"); CharSpriteName = "7_2_sad"; break; }
                case 317: { CharSpriteName = "7_sad"; break; }
                case 324: { CharSpriteName = "2_standart"; break; }
                case 329: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                case 330: { imgName = "Small_Apartment_Kitchen"; CharSpriteName = "6_sleepy"; break; }
                case 332: { CharSpriteName = "7_sad"; break; }
                #endregion

                #region Сцена "Дождь"
                case 334: { imgName = "Street_Summer_Rain"; CharSpriteName = "void"; BackPlay("rain"); break; }
                case 337: { imgName = "dark"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Исходный код"
                case 343: { imgName = "Small_Apartment_Kitchen"; SoundPlay("door"); break; }
                case 346: { imgName = "Bedroom_Day"; break; }
                case 347: { CharSpriteName = "6_sleepy"; break; }
                case 348: { CharSpriteName = "void"; break; }
                case 365: { imgName = "dark"; break; }
                #endregion

                #region Сцена "И иснова доброе утро"
                case 366: { imgName = "Kitchen_Day"; BackPlay("backMus3"); break; }
                case 367: { CharSpriteName = "2_standart"; break; }
                case 369: { CharSpriteName = "3_shocked"; break; }
                case 373: { CharSpriteName = "7_sad"; break; }
                case 377: { CharSpriteName = "8_happy"; break; }
                case 378: { CharSpriteName = "4_2_love"; break; }
                case 379: { CharSpriteName = "2_standart"; break; }
                case 380: { CharSpriteName = "4_2_love"; break; }
                case 381: { CharSpriteName = "2_standart"; break; }
                case 383: { imgName = "dark"; CharSpriteName = "void"; BackPlay("silent"); break; }
                #endregion

                #region Сцена "Неприятный звонок"
                case 391: { imgName = "Livingroom_Dark"; BackPlay("iphone"); break; }
                case 392: { BackPlay("silent"); break; }
                case 397: { BackPlay("phone_drop"); break; }
                case 399: { BackPlay("silent"); break; }
                case 402: { imgName = "dark"; break; }
                #endregion

                #region Сцена "Победа"
                case 404: { imgName = "Small_Apartment_Kitchen"; SoundPlay("door"); break; }
                case 405: { BackPlay("backMus3"); CharSpriteName = "8_2_happy"; Anim_StandUp(); break; }
                case 409: { CharSpriteName = "8_happy"; break; }
                case 411: { CharSpriteName = "2_standart"; break; }
                case 413: { CharSpriteName = "3_shocked"; break; }
                case 414: { CharSpriteName = "4_love"; break; }
                case 417: { CharSpriteName = "3_shocked"; break; }
                case 419: { CharSpriteName = "7_sad"; break; }
                case 420: { CharSpriteName = "4_2_love"; break; }
                case 425: { CharSpriteName = "7_sad"; break; }
                case 428: { CharSpriteName = "3_shocked"; break; }
                case 430: { CharSpriteName = "4_2_love"; break; }
                case 432: { CharSpriteName = "3_shocked"; break; }
                case 434: { CharSpriteName = "8_happy"; break; }
                case 435: { CharSpriteName = "7_sad"; break; }
                case 438: { CharSpriteName = "4_love"; break; }
                case 441: { imgName = "dark"; CharSpriteName = "void"; break; }
                case 449: { BackPlay("silent"); break; }
                #endregion

                case 450:
                    {
                        Process.Start("Annoying Haru.exe");
                        BackPlay("off");
                        Application.Current.Shutdown();
                        break;
                    }
            }

            story += (textsArr[textID] + "\n");

            characterHolder.Source = new BitmapImage(new Uri(@"character/" + CharSpriteName + ".png", UriKind.Relative));
            picHolder.Source = new BitmapImage(new Uri(@"images/" + imgName + ".jpg", UriKind.Relative));

            AnimaText();
        }
        #endregion

        #region Helpers Methods
        private void SetTextColor()
        {
            if (textsArr[textID].Contains("Хару:")) mainText.Foreground = Brushes.DeepPink;
            else if (textsArr[textID].Contains(nameOfGamer + ":")) mainText.Foreground = Brushes.Aqua;
            else mainText.Foreground = Brushes.White;
        }

        #endregion

        #region Text In/From RichTextBox
        private string StringFromRTB(RichTextBox rtb)   //Получить текст из RichTextBox
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }
        private void StringInRTB(string received_str, RichTextBox rtb)   //Записать текст в RichTextBox
        {
            FlowDocument objFdoc = new FlowDocument();
            Paragraph objPara1 = new Paragraph();
            objPara1.Inlines.Add(new Run(received_str));
            objFdoc.Blocks.Add(objPara1);
            rtb.Document = objFdoc;
        }
        #endregion

        #region Animations
        private void AnimaText()    //Анимация текста
        {
            dispatcherTimer.Start();
        }
        private void Anim_GoOut()   //Анимация ухода персонажа за кадр в правую сторону
        {
            TranslateTransform trans = new TranslateTransform();
            characterHolder.RenderTransform = trans;

            DoubleAnimation animX = new DoubleAnimation(0, 1400, TimeSpan.FromSeconds(animationTime));
            trans.BeginAnimation(TranslateTransform.XProperty, animX);
        }
        private void Anim_StandUp() //Анимация появления персонажа снизу экрана
        {
            TranslateTransform trans = new TranslateTransform();
            characterHolder.RenderTransform = trans;

            DoubleAnimation animY = new DoubleAnimation(Math.Pow(10, 3), 0, TimeSpan.FromSeconds(animationTime));
            trans.BeginAnimation(TranslateTransform.YProperty, animY);
        }
        private void Anim_ClosingIn(int min, int max)   //Анимация приближения/отдаления персонажа (увеличение/отдаления спрайта)
        {
            DoubleAnimation anim = new DoubleAnimation(min, max, TimeSpan.FromSeconds(animationTime));
            characterHolder.BeginAnimation(HeightProperty, anim);
        }
        #endregion

        #region Sounds
        private void BackPlay(string soundName)
        {
            WriteToFile(@"DataBackgroundMusic.txt", soundName);

            if (textID == 1)
                Process.Start("BackroundMusicApp.exe");     //Запускает отдельное приложение, воспроизводящее фоновую музыку
        }
        private void SoundPlay(string soundName)
        {
            try
            {
                SoundPlayer sp = new SoundPlayer();
                sp.SoundLocation = @"sounds/" + soundName + ".wav";
                sp.Load();
                sp.Play();
            }
            catch (Exception er)
            {
                Console.WriteLine("Exception: " + er.Message);
            }
        }
        #endregion

        #region Buttons
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveBtn.IsEnabled = false; LoadBtn.IsEnabled = false;

            textID = 0;
            animationTime = 0.001f;
            Anim_StandUp();
            CharSpriteName = "void";

            int currentTextID = Convert.ToInt32(ReadFromFile(@"DataSaveTextID.txt"));

            BackPlay("off");
            Thread.Sleep(200);

            if((currentTextID < 0) || (currentTextID > maxTextsCount))
            {
                MessageBox.Show("Похоже в файл сохранения были внесены изменения и теперь всё сломалось...", "Повреждённый текст!", MessageBoxButton.OK, MessageBoxImage.Error);
                WriteToFile(@"DataSaveTextID.txt", Convert.ToString("1"));
                Application.Current.Shutdown();
            }

            story = "";

            for (int i = 0; i < currentTextID; i++)
                PlotFlow();

            animationTime = 1f;

            Thread.Sleep(200);

            //Учет загрузки при разных исходных размерах окна
            if (_window.WindowState == WindowState.Normal)
            {
                mainText.FontSize = 12;
                characterHolder.Margin = new Thickness(135, 50, 135, -50);
            }
            else
            {
                mainText.FontSize = 21;
                characterHolder.Margin = new Thickness(220, 20, 220, -50);
            }

            SoundPlay("loadSound");
        }
        private void confirmNameBtn_Click(object sender, RoutedEventArgs e)
        {
            SoundPlay("sound_btn");

            yourName.Foreground = Brushes.White;
            nameOfGamer = StringFromRTB(yourName).Trim();               //Получение указанного имени

            string engAlph = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string rusAlph = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            byte threeSym = 0;
            string testingName = nameOfGamer.ToUpper();

            for (byte i = 0; i < nameOfGamer.Length; i++)
                if (engAlph.Contains(testingName[i]) || rusAlph.Contains(testingName[i]))
                    threeSym++;

            if (nameOfGamer.Length < 3)
            {
                nameOfGamer = "Акира";
                StringInRTB(nameOfGamer, yourName);
                MessageBox.Show("Слишком мало символов!", "Некорректное имя!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(nameOfGamer.Length > 16)
            {
                nameOfGamer = "Акира";
                StringInRTB(nameOfGamer, yourName);
                MessageBox.Show("Слишком много символов!", "Некорректное имя!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (nameOfGamer == null)
            {
                nameOfGamer = "Акира";
                StringInRTB(nameOfGamer, yourName);
            }
            else if (threeSym < 3)
            {
                nameOfGamer = "Акира";
                StringInRTB(nameOfGamer, yourName);
                MessageBox.Show("В имени должно быть минимуму 3 буквы!", "Некорректное имя!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else { yourName.Foreground = Brushes.LightGreen; }

            WriteToFile(@"DataSaveName.txt", nameOfGamer);
        }
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            SoundPlay("sound_btn");
            textID = 0;
            PlotFlow();
        }
        private void moveNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SoundPlay("sound_btn");

            SaveBtn.IsEnabled = true; LoadBtn.IsEnabled = true;

            PlotFlow();
            storyText.Margin = new Thickness(267, -900, 267, 0);
        }
        private void moveBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SoundPlay("sound_btn");
            StringInRTB(story, storyText);
            storyText.Margin = new Thickness(267, 30, 267, 120);
            storyText.ScrollToEnd();
        }
        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            BackPlay("off");
            Application.Current.Shutdown();
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveBtn.IsEnabled = false; LoadBtn.IsEnabled = false;

            mainText.Height = 100;
            WriteToFile(@"DataSaveTextID.txt", Convert.ToString(textID));

            Thread.Sleep(200);

            SoundPlay("saveSound");
            StringInRTB("GAME WAS SAVED", mainText);
        }
        private void HideModeBtn_Click(object sender, RoutedEventArgs e)
        {
            SoundPlay("sound_btn");
            _window.WindowState = WindowState.Minimized;
        }
        private void ResizeModeBtn_Click(object sender, RoutedEventArgs e)
        {
            SoundPlay("sound_btn");
            if (_window.WindowState == WindowState.Normal)
            {
                _window.WindowState = WindowState.Maximized;
                mainText.FontSize = 21;
                characterHolder.Margin = new Thickness(135, 0, 135, -60);
            }
            else
            {
                _window.WindowState = WindowState.Normal;
                mainText.FontSize = 12;
                characterHolder.Margin = new Thickness(220, 0, 220, -300);
            }
        }
        #endregion

        #region Read/Write File
        private void WriteToFile(string gettingFileName, string saveContent)    //Запись данных в указанный файл
        {
            try
            {
                StreamWriter sw = new StreamWriter(gettingFileName);
                sw.WriteLine(saveContent);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private string ReadFromFile(string gettingFileName)     //Вывод данных из указанного файла
        {
            string file_data = "none";
            try
            {
                StreamReader sr = new StreamReader(gettingFileName);
                file_data = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return file_data;
        }

        #endregion
    }
}
