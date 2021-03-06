﻿using System;
using System.Collections.Generic;

namespace Yutaka.QuickBooks
{
	public class BillRet
	{
		public string TxnID; // required
		public DateTime TimeCreated; // required
		public DateTime TimeModified; // required
		public string EditSequence; // required
		public int? TxnNumber;
		public VendorRef VendorRef;
		public VendorAddress VendorAddress;
		public APAccountRef APAccountRef;
		public DateTime TxnDate; // required
		public DateTime? DueDate;
		public decimal AmountDue; // required
		public CurrencyRef CurrencyRef;
		public decimal? ExchangeRate;
		public decimal? AmountDueInHomeCurrency;
		public string RefNumber;
		public TermsRef TermsRef;
		public string Memo;
		public bool? IsTaxIncluded;
		public SalesTaxCodeRef SalesTaxCodeRef;
		public bool? IsPaid;
		public string ExternalGUID;
		public LinkedTxn LinkedTxn;
		public decimal? OpenAmount;
		//Changes 5/20
		public DataExtRet DataExtRet;
		public List<Bill_ExpenseLineRet> ExpenseLines;

		public BillRet()
		{
			VendorRef = new VendorRef();
			VendorAddress = new VendorAddress();
			APAccountRef = new APAccountRef();
			CurrencyRef = new CurrencyRef();
			TermsRef = new TermsRef();
			SalesTaxCodeRef = new SalesTaxCodeRef();
			LinkedTxn = new LinkedTxn();
			DataExtRet = new DataExtRet();
			ExpenseLines = new List<Bill_ExpenseLineRet>();
		}
	}
}