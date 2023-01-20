using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace TarkovFLeaM // Note: actual namespace depends on the project name.
{
    internal class Class1 {
        private List<string> paths;
        private List<string> namePaths;
        private List<string> itemsToBeUpdated;
        private List<Item> _items;
        private IWebDriver driver;
        private Actions actions;

        public static void Main(string[] args) {
            Class1 class1 = new Class1();
            class1.Run();
        }

        public void ApplySettings(ChromeOptions options, bool headless) {
            if (headless) options.AddArgument("--headless");
            options.AddArgument("--disable-images");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-plugins-discovery");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-accelerated-jpeg-decoding");
            options.AddArgument("--disable-accelerated-video-decode");
            options.AddArgument("--disable-cache");
        }

        public void Run() {
            paths = new List<string>();
            namePaths = new List<string>();

            ChromeOptions options = new ChromeOptions();
            ApplySettings(options, true);


            driver = new ChromeDriver(options);
            actions = new Actions(driver);

            AddPotentialPaths();
            AddPotentialPathsForName();

            Scan();
            Console.WriteLine("FORMING ITEMS");
            FormItems();
        }

        public void GetIDs() {
            Console.WriteLine("ID TEST");
            IWebElement searchField;

            foreach (Item item in _items) {
                Console.WriteLine("Item Check ID");
                driver.Navigate().GoToUrl("https://db.sp-tarkov.com/search");
                searchField = driver.FindElement(By.Id("search-autocomplete"));
                searchField.SendKeys(item.name.Remove(item.name.Length - 1));

                string id = null;
                int emptyInRow = 4;
                int retry = 1;
                int trycount = 0;
                
                while (id == null) {
                    trycount++;
                    Thread.Sleep(325);
                    actions.MoveToElement(searchField, 0, 50).Click().Perform();
                    Console.WriteLine("Trying to enter");
                    
                    Thread.Sleep(625);
                    id = GetIdOutOfDataList();
                    if(id != null)break;
                    
                    SendBackSpace(3-retry, searchField);

                    if (searchField.GetDomProperty("value") == "") {
                        emptyInRow -= 1;

                        if (emptyInRow < 2 && retry > 0) {
                            searchField.SendKeys(item.name.Remove(item.name.Length - 1));
                            retry--;
                            emptyInRow = 4;
                        }
                        if (emptyInRow < 0) break;
                    }
                    else emptyInRow = 4;
                }
                
                if(id == null)continue;
                
                id = GetIdOutOfDataList();
                Console.WriteLine("Item found ID: " + id);


                item.id = id;
            }
        }

        public void SendBackSpace(int amount, IWebElement field) {
            for (int i = 0; i < amount; i++) {
                field.SendKeys(Keys.Backspace);
            }
        }

        public string GetIdOutOfDataList() {
            string id = "";

            try {
                id = driver.FindElement(By.ClassName("string-value")).Text;
            }
            catch (NoSuchElementException e) {
                Console.WriteLine("ID Not Found");
                return null;
            }

            return id;
        }

        public void Scan() {
            _items = new List<Item>();
            itemsToBeUpdated = new List<string>();
            Console.WriteLine("ADDING KEYS");
            AddKeys();
            Console.WriteLine("ADDING MEDS");
            AddMeds();
            Console.WriteLine("ADDING BARTER");
            AddBarter();
            Console.WriteLine("ADDING MAGS");
            //AddMag();
            Console.WriteLine("ADDING AMMO");
            //AddAmmo();
            Console.WriteLine("ADDING FOOD");
            AddProvisions();
            Console.WriteLine("ADDING CONTAINER");
            AddContainer();
        }

        //Mag Update
        public void AddMag() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/magazines");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Food Update
        public void AddProvisions() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/provisions");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Ammo Update
        public void AddAmmo() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/ammo");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Container Update
        public void AddContainer() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/containers");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Keys Update
        public void AddKeys() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/keys");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetDomProperty("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Meds Update
        public void AddMeds() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/meds");
            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("item")).ToList();

            CheckAdress(filteredLinks);
        }

        //Barter Update
        public void AddBarter() {
            driver.Navigate().GoToUrl("https://tarkov-market.com/tag/barter");

            ScrollDownUntilEnd(driver);

            IEnumerable<IWebElement> links = driver.FindElements(By.TagName("a"));
            List<IWebElement> filteredLinks = links.Where(x => x.GetAttribute("href").Contains("/item/")).ToList();

            CheckAdress(filteredLinks);
        }

        public void ScrollDownUntilEnd(IWebDriver driver) {
            var button = TryGetButton(driver);
            button.Click();
            long lastPosition = 0;
            int i = 0;

            while (i < 300) {
                bool isOnLimit = (lastPosition == CheckScrollLimit(driver));
                if (isOnLimit) {
                    i++;
                    Thread.Sleep(15);
                }
                else i = 0;

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("window.scrollBy(0,250)");
                lastPosition = CheckScrollLimit(driver);
            }

            Console.WriteLine("Reading Has Ended*-");
        }

        public IWebElement TryGetButton(IWebDriver driver) {
            IWebElement button;

            try {
                button = driver.FindElement(By.XPath("//button[@class='big bold w-100 text-center py-10']"));
            }
            catch (NoSuchElementException noElement) {
                Console.WriteLine("Button not found");
                return null;
            }

            return button;
        }

        public long CheckScrollLimit(IWebDriver driver) {
            return (long)((IJavaScriptExecutor)driver).ExecuteScript("return window.pageYOffset;");
        }

        public void CheckAdress(IEnumerable<IWebElement> links) {
            Console.WriteLine("Link amount : " + links.Count());

            string lastAddress = "";
            foreach (IWebElement link in links.ToList()) {

                bool sameitemfound = false;
                string adress = link.GetDomProperty("href");
                
                if (lastAddress != "") {
                    if (adress == lastAddress) {
                        Console.WriteLine("Same Address Found");
                        lastAddress = "";
                        continue;
                    }
                }
                
                
                Console.WriteLine("Address Found: " + adress);
                itemsToBeUpdated.Add(adress);

                lastAddress = adress;
            }
        }

        public void FormItems() {
            string dataToSave = "";
            foreach (string itemT in itemsToBeUpdated) {
                TryGoToUrl(itemT);
                Item item = FormItem();

                if (item == null || item.name == String.Empty || item.name == " ") {
                    continue;
                }

                Console.WriteLine("Item Added: " + item.name + " " + item.name.Length);
                _items.Add(item);
            }

            ChromeOptions options = new ChromeOptions();
            ApplySettings(options, false);
            driver = new ChromeDriver(options);
            actions = new Actions(driver);

            Console.WriteLine("Item Count: " + _items.Count);
            GetIDs();

            foreach (Item item in _items) {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Price: " + item.price);
                Console.WriteLine("Name: " + item.name);
                Console.WriteLine("Item ID: " + item.id);

                if (item.id == "null") {
                    Console.WriteLine("Item Removed Since No ID");
                    continue;
                }

                dataToSave += "p:" + item.price + "\n" + "id:" + item.id + "\n" + "\n";
            }


            using (StreamWriter writetext = new StreamWriter("Data.txt")) {
                writetext.WriteLine(dataToSave);
            }

            Console.WriteLine(itemsToBeUpdated.Count + " Items Found!");
        }

        public bool TryGoToUrl(string adress) {
            try {
                driver.Navigate().GoToUrl(adress);
            }
            catch (Exception e) {
                Console.WriteLine("An error occured while trying to connect to an adress");
                return false;
            }

            return true;
        }

        public void AddPotentialPaths() {
            paths.Add(
                "//div/div/div/div/div[2]/div[2]/div[3]/div[1]/div[2]/div/div[2]/div[1]/div/div[2]/div/div/div[1]");
            paths.Add("//div/div/div/div/div[2]/div[2]/div[3]/div[1]/div[2]/div/div[1]/div[1]/div[1]/div/div/div[2]");
        }

        public void AddPotentialPathsForName() {
            namePaths.Add("//div/div/div/div/div[2]/div[2]/div[3]/div[1]/div[1]/h1");
        }

        public Item FormItem() {
            Item item = new Item(CheckAllDefinedPathsForName(), "null",
                CheckAllDefinedPathsForPrice());

            if (item.name == null) return null;

            return item;
        }

        public string CheckAllDefinedPathsForPrice() {
            foreach (string path in paths) {
                try {
                    IWebElement priceField =
                        driver.FindElement(By.XPath(path));
                    if (priceField != null) {
                        string price = priceField.Text;

                        string priceFixer = Regex.Replace(price, @"[^\u0000-\u007F]+", string.Empty);
                        return priceFixer;
                    }
                }
                catch (NoSuchElementException e) {
                    continue;
                }
            }

            return null;
        }

        public string CheckAllDefinedPathsForName() {
            foreach (string path in namePaths) {
                try {
                    IWebElement priceField =
                        driver.FindElement(By.XPath(path));
                    if (priceField != null) {
                        string name = priceField.Text;
                        name = Regex.Replace(name, @"&nbsp;", " ");
                        name = Regex.Replace(name, @"[^\u0000-\u007F]+", string.Empty);
                        return name;
                    }
                }
                catch (NoSuchElementException e) {
                    continue;
                }
            }

            return null;
        }
    }
}