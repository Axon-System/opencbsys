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
using OpenCBS.CoreDomain.Contracts.Loans.Installments;
using OpenCBS.CoreDomain.Contracts.Loans.LoanRepayment.Interfaces;
using OpenCBS.Shared;


namespace OpenCBS.CoreDomain.Contracts.Loans.LoanRepayment.Repayment.FeesRepayment
{
    [Serializable]
    public class AutomaticMethod : IFeesRepayment
    {
        public void Repay(Installment pInstallment, ref OCurrency pAmountPaid, ref OCurrency pFeesEvent)
        {
            if (AmountComparer.Compare(pAmountPaid, pInstallment.FeesUnpaid) > 0)
            {
                pFeesEvent += pInstallment.FeesUnpaid;
                pAmountPaid -= pInstallment.FeesUnpaid;
                pInstallment.PaidFees += pInstallment.FeesUnpaid;
                pInstallment.FeesUnpaid = 0;

            }
            else
            {
                pFeesEvent += pAmountPaid;
                pInstallment.PaidFees += pAmountPaid;
                pInstallment.FeesUnpaid -= pAmountPaid;
                pAmountPaid = 0;
            }
        }
    }
}
