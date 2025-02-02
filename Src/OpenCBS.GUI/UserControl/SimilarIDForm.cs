﻿// Octopus MFS is an integrated suite for managing a Micro Finance Institution: 
// clients, contracts, accounting, reporting and risk
// Copyright © 2006,2007 OCTO Technology & OXUS Development Network
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//
// Website: http://www.opencbs.com
// Contact: contact@opencbs.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCBS.CoreDomain.Clients;

namespace OpenCBS.GUI.UserControl
{
    public partial class SimilarIDForm : Form
    {
        public SimilarIDForm(List<Person> pPersons)
        {
            InitializeComponent();
            _InitializeList(pPersons);
        }
        private void _InitializeList(List<Person> pPersons)
        {
            foreach (Person person in pPersons)
            {
                ListViewItem li = new ListViewItem(person.IdentificationData);
                li.SubItems.Add(person.FirstName);
                li.SubItems.Add(person.LastName);
                li.Tag = person;
                listViewPersons.Items.Add(li);
            }

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
