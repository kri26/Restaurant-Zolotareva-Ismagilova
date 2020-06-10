﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using RestaurantBusinessLogic.Interfaces;
using RestaurantBusinessLogic.ViewModels;
using RestaurantBusinessLogic.BindingModels;
using RestaurantBusinessLogic.BusinessLogics;

namespace RestaurantView
{
    public partial class FormCreateOrder : Form
    {
        [Dependency]
        public new IUnityContainer Container { get; set; }
        private readonly IDishLogic logicP;
        private readonly MainLogic logicM;
        private readonly IRequestLogic requestLogic;
        private Dictionary<int, (string, int, bool)> requestFoods;
        public int ID { set { Id = value; } }
        private int? Id;
        private readonly ISupplierLogic supplierLogic;

        public FormCreateOrder(IDishLogic logicP, MainLogic logicM,
            IRequestLogic requestLogic, ISupplierLogic supplierLogic)
        {
            InitializeComponent();
            this.logicP = logicP;
            this.logicM = logicM;
            this.requestLogic = requestLogic;
            this.supplierLogic = supplierLogic;
        }

        private void FormCreateOrder_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
            try
            {
                //Логика загрузки списка компонент в выпадающий список
                List<DishViewModel> listDishses = logicP.Read(null);
                if (listDishses != null)
                {
                    comboBoxProduct.DisplayMember = "DishName";
                    comboBoxProduct.ValueMember = "Id";
                    comboBoxProduct.DataSource = listDishses;
                    comboBoxProduct.SelectedItem = null;
                }

                if (Id.HasValue)
                {
                    RequestViewModel request = requestLogic.Read(new RequestBindingModel
                    {
                        Id = Id.Value
                    })?[0];
                    if (request != null)
                    {
                        requestFoods = request.Foods;
                    }
                }
                else
                {
                    requestFoods = new Dictionary<int, (string, int, bool)>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
               MessageBoxIcon.Error);
            }
        }

        private void CalcSum()
        {
            if (comboBoxProduct.SelectedValue != null && !string.IsNullOrEmpty(textBoxCount.Text))
            {
                try
                {
                    int id = Convert.ToInt32(comboBoxProduct.SelectedValue);
                    DishViewModel product = logicP.Read(new DishBindingModel
                    {
                        Id = id
                    })?[0];
                    int count = Convert.ToInt32(textBoxCount.Text);
                    textBoxSum.Text = (count * product?.Price ?? 0).ToString();
                    foreach (var p in product.DishFoods)
                    {
                        requestFoods.Add(p.Key, (product.DishFoods[p.Key].Item1, product.DishFoods[p.Key].Item2, false));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TextBoxCount_TextChanged(object sender, EventArgs e)
        {
            CalcSum();
        }

        private void ComboBoxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcSum();
        }

        private void LoadSuppliers()
        {
            try
            {
                List<SupplierViewModel> suppliersList = supplierLogic.Read(null);
                if (suppliersList != null)
                {
                    comboBoxSupplier.DisplayMember = "Login";
                    comboBoxSupplier.ValueMember = "Id";
                    comboBoxSupplier.DataSource = suppliersList;
                    comboBoxSupplier.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка загрузки списка поставщиков",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxCount.Text))
            {
                MessageBox.Show("Заполните поле Количество", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBoxProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите продукт", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxSupplier.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                logicM.CreateOrder(new OrderBindingModel
                {
                    DishId = Convert.ToInt32(comboBoxProduct.SelectedValue),
                    Count = Convert.ToInt32(textBoxCount.Text),
                    Sum = Convert.ToDecimal(textBoxSum.Text)
                });
                logicM.CreateOrUpdateRequest(new RequestBindingModel
                {
                    Id = Id,
                    SupplierId = Convert.ToInt32(comboBoxSupplier.SelectedValue),
                    Foods = requestFoods
                });
                MessageBox.Show("Сохранение прошло успешно", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}