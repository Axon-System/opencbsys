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
using OpenCBS.CoreDomain.Events;
using OpenCBS.Enums;
using OpenCBS.CoreDomain.Products;
using OpenCBS.CoreDomain.EconomicActivities;

namespace OpenCBS.CoreDomain.Accounting
{
    [Serializable]
    public class ContractAccountingRule : IAccountingRule
    {
        #region IAccountingRule Members
        public int Id { get; set; }
        public Account DebitAccount { get; set; }
        public Account CreditAccount { get; set; }
        public bool Deleted { get; set; }
        public OBookingDirections BookingDirection { get; set; }
        public EventType EventType { get; set; }
        public EventAttribute EventAttribute { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        #endregion

        public ContractAccountingRule()
        {
            PaymentMethod = new PaymentMethod();
        }
        public OProductTypes ProductType { get; set; }
        public LoanProduct LoanProduct { get; set; }
        public Currency Currency { get; set; }
        public ISavingProduct SavingProduct { get; set; }
        public OClientTypes ClientType { get; set; }
        public EconomicActivity EconomicActivity { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public ContractAccountingRule Copy()
        {
            return (ContractAccountingRule) MemberwiseClone();
        }
    }
}
