﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using MaterialDesignMessageBox;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Controls;
using System.Linq;

namespace MacoApp
{
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        WorkGoogleDrive workGoogle = new WorkGoogleDrive();

        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\Furnapp.db";
        static string pathSaveFolder = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\SaveDB";
        static string path2 = pathSaveFolder+ "\\Furnapp.db";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            
        }

        // при загрузке окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            db.Database.EnsureCreated();
            // загружаем данные из БД
            db.Elements.Load();
            // и устанавливаем данные в качестве контекста
            DataContext = db.Elements.Local.ToObservableCollection();
            
            //ButtonSearch.IsEnabled = false;
            //TextBoxSearch.IsEnabled = false;
        }

        // добавление
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowElementAdd WindowElementAdd = new WindowElementAdd(new Element());
                if (WindowElementAdd.ShowDialog() == true)
                {
                    Element Element = WindowElementAdd.Element;
                    db.Elements.Add(Element);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                MaterialMessageBox.ShowDialog("Судя по всему не верно заполнено одно из значений");
                return;
            }
            //backup(pathSaveFolder);
        }
        // редактирование
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Element? element = elementsList.SelectedItem as Element;
            // если ни одного объекта не выделено, выходим
            if (element is null)
            {
                MaterialMessageBox.ShowDialog("Не выбрана строка");
                return;
            }
            try
            {
                WindowElementAdd WindowElementAdd = new WindowElementAdd(new Element
                {
                    Id = element.Id,
                    Name_Furn = element.Name_Furn,
                    Title = element.Title,
                    Article = element.Article,
                    Quantity = element.Quantity,
                    System = element.System,
                    Side = element.Side,
                    FFH_before = element.FFH_before,
                    FFH_after = element.FFH_after,
                    FFB_before = element.FFB_before,
                    FFB_after = element.FFB_after,
                    Lower_loop = element.Lower_loop,
                    Micro_ventilation = element.Micro_ventilation,
                    Rotation = element.Rotation
                });

                if (WindowElementAdd.ShowDialog() == true)
                {
                    // получаем измененный объект
                    element = db.Elements.Find(WindowElementAdd.Element.Id);
                    if (element != null)
                    {
                        element.Name_Furn = WindowElementAdd.Element.Name_Furn;
                        element.Title = WindowElementAdd.Element.Title;
                        element.Article = WindowElementAdd.Element.Article;
                        element.Quantity = WindowElementAdd.Element.Quantity;
                        element.System = WindowElementAdd.Element.System;
                        element.Side = WindowElementAdd.Element.Side;
                        element.FFH_before = WindowElementAdd.Element.FFH_before;
                        element.FFH_after = WindowElementAdd.Element.FFH_after;
                        element.FFB_before = WindowElementAdd.Element.FFB_before;
                        element.FFB_after = WindowElementAdd.Element.FFB_after;
                        element.Lower_loop = WindowElementAdd.Element.Lower_loop;
                        element.Micro_ventilation = WindowElementAdd.Element.Micro_ventilation;
                        element.Rotation = WindowElementAdd.Element.Rotation;
                        db.SaveChanges();
                        elementsList.Items.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                MaterialMessageBox.ShowDialog("Ой, что-то пошло не так...");
                return;
            }
            //backup(pathSaveFolder);
        }
        // удаление
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Element? element = elementsList.SelectedItem as Element;
            // если ни одного объекта не выделено, выходим
            if (element is null)
            {
                MaterialMessageBox.ShowDialog("Не выбрана строка для удаления");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Точно удалить???", "Точно?", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    db.Elements.Remove(element);
                    db.SaveChanges();
                    break;
                case MessageBoxResult.No:
                    break;
            }
            //backup(pathSaveFolder);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EntryiWindow entryiWindow = new EntryiWindow();
            entryiWindow.Show();
            this.Close();
            backup(pathSaveFolder);
        }

        public void backup(string strDestination)
        {
            if (!Directory.Exists(pathSaveFolder)) Directory.CreateDirectory(pathSaveFolder); //Создаем папку для хранения измененной БД
            try
            {
                FileInfo fileInf = new FileInfo(path2);
                if (fileInf.Exists)
                {
                    fileInf.Delete(); // Удаляем старый файл
                    File.Copy(path, path2); //Копируем в новую папку БД. чтобы оттуда скопировать в Google Drive
                }
                else 
                {
                    File.Copy(path, path2);
                }
                //Передем файл в стрим для передачи в Google Drive
                FileStream fs = new FileStream(path2,FileMode.Open);
                //Передаем файл в Google Drive
                workGoogle.UpdateFile(fs, "1xfKqKlY6V-k6mjQlnKit8qQ7t_Z4_eSb", "Data Base File/db");
            }
            catch (System.Exception)
            {
                return;
            }
        }

    }
}