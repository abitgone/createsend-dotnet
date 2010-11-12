﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace createsend_dotnet
{
    public class List
    {
        private string _listID;

        public List(string listID)
        {
            _listID = listID;
        }

        public static string Create(string clientID, string title, string unsubscribePage, bool confirmedOptIn, string confirmationSuccessPage)
        {
            string json = HttpHelper.Post(string.Format("/lists/{0}.json", clientID), null, JavaScriptConvert.SerializeObject(
                new ListDetail() { Title = title, UnsubscribePage = unsubscribePage, ConfirmedOptIn = confirmedOptIn, ConfirmationSuccessPage = confirmationSuccessPage })
                );
            return JavaScriptConvert.DeserializeObject<string>(json);
        }

        public void Update(string title, string unsubscribePage, bool confirmedOptIn, string confirmationSuccessPage)
        {
            HttpHelper.Put(string.Format("/lists/{0}.json", _listID), null, JavaScriptConvert.SerializeObject(
                new ListDetail() { Title = title, UnsubscribePage = unsubscribePage, ConfirmedOptIn = confirmedOptIn, ConfirmationSuccessPage = confirmationSuccessPage })
                );
        }

        public ListDetail Details()
        {
            string json = HttpHelper.Get(string.Format("/lists/{0}.json", _listID), null);
            return JavaScriptConvert.DeserializeObject<ListDetail>(json);
        }

        public void Delete()
        {
            HttpHelper.Delete(string.Format("/lists/{0}.json", _listID), null);
        }

        public string CreateCustomField(string fieldName, CustomFieldDataType dataType, List<string> options)
        {
            string json = HttpHelper.Post(string.Format("/lists/{0}/customfields.json", _listID), null, JavaScriptConvert.SerializeObject(
                new ListCustomFieldForAdd() { FieldName = fieldName, DataType = dataType.ToString(), Options = options })
                );
            return JavaScriptConvert.DeserializeObject<string>(json);
        }

        public void DeleteCustomField(string customFieldKey)
        {
            HttpHelper.Delete(string.Format("/lists/{0}/customfields/{1}.json", _listID, System.Web.HttpUtility.UrlEncode(customFieldKey)), null);
        }

        public IEnumerable<ListCustomField> CustomFields()
        {
            string json = HttpHelper.Get(string.Format("/lists/{0}/customfields.json", _listID), null);
            return JavaScriptConvert.DeserializeObject<ListCustomField[]>(json);
        }

        public IEnumerable<BasicSegment> Segments()
        {
            string json = HttpHelper.Get(string.Format("/lists/{0}/segments.json", _listID), null);
            return JavaScriptConvert.DeserializeObject<BasicSegment[]>(json);
        }

        public ListStats Stats()
        {
            string json = HttpHelper.Get(string.Format("/lists/{0}/stats.json", _listID), null);
            return JavaScriptConvert.DeserializeObject<ListStats>(json);
        }

        public PagedCollection<Subscriber> Active(DateTime fromDate, int page, int pageSize, string orderField, string orderDirection)
        {
            return GenericPagedSubscriberGet("active", fromDate, page, pageSize, orderField, orderDirection);
        }

        public PagedCollection<Subscriber> Unsubscribed(DateTime fromDate, int page, int pageSize, string orderField, string orderDirection)
        {
            return GenericPagedSubscriberGet("unsubscribed", fromDate, page, pageSize, orderField, orderDirection);
        }

        public PagedCollection<Subscriber> Bounced(DateTime fromDate, int page, int pageSize, string orderField, string orderDirection)
        {
            return GenericPagedSubscriberGet("bounced", fromDate, page, pageSize, orderField, orderDirection);
        }

        private PagedCollection<Subscriber> GenericPagedSubscriberGet(string type, DateTime fromDate, int page, int pageSize, string orderField, string orderDirection)
        {
            NameValueCollection queryArguments = new NameValueCollection();
            queryArguments.Add("date", fromDate.ToString("yyyy-MM-dd HH:mm:ss"));
            queryArguments.Add("page", page.ToString());
            queryArguments.Add("pagesize", pageSize.ToString());
            queryArguments.Add("orderfield", orderField);
            queryArguments.Add("orderdirection", orderDirection);

            string json = HttpHelper.Get(string.Format("/lists/{0}/{1}.json", _listID, type), queryArguments);
            return JavaScriptConvert.DeserializeObject<PagedCollection<Subscriber>>(json);
        }
    }
}