using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vehicle.Allocation.Abstract;
using Vehicle.Allocation.Models;

namespace Vehicle.Allocation.Controllers
{
    public class MainController : Controller
    {

        [HttpPost]
        public string TryLogin()
        {
            string username = Request["username"];
            string password = Request["password"];

            Entities entity = new Entities();

            vaAccount account = entity.vaAccounts.Where(a => a.Username == username).FirstOrDefault();

            if (account != null)
            {
                if (account.Password == password)
                {
                    Account json = new Account()
                    {
                        pk = account.PK,
                        type = account.vaAccountType.Value.ToLower(),
                        username = account.Username,
                        password = account.Password,
                        field = account.Field,
                        name = account.Name,
                        surname = account.Surname
                    };

                    return new JavaScriptSerializer().Serialize(json);
                }
                else
                {
                    return "wrong_password";
                }
            }
            else
            {
                return "wrong_username";
            }
        }

        [HttpPost]
        public string NewUser()
        {
            string name = Request["name"];
            string surname = Request["surname"];
            string username = Request["username"];
            string password = Request["password"];
            string field = Request["field"];

            Entities entity = new Entities();

            vaAccount account = entity.vaAccounts.Where(a => a.Username == username).FirstOrDefault();

            if (account == null)
            {
                account = entity.vaAccounts.Where(a => a.Field == field).FirstOrDefault();

                if (account == null)
                {
                    account = new vaAccount()
                    {
                        TypeFK = 1,
                        Name = name,
                        Surname = surname,
                        Username = username,
                        Password = password,
                        Field = field
                    };

                    entity.vaAccounts.Add(account);
                    entity.SaveChanges();

                    return account.PK.ToString();
                }
                else
                {
                    return "email_exists";
                }
            }
            else
            {
                return "username_exists";
            }
        }

        [HttpPost]
        public string NewFirm()
        {
            string name = Request["name"];
            string username = Request["username"];
            string password = Request["password"];
            string field = Request["field"];

            Entities entity = new Entities();

            vaAccount account = entity.vaAccounts.Where(a => a.Username == username).FirstOrDefault();

            if (account == null)
            {
                account = new vaAccount()
                {
                    TypeFK = 2,
                    Name = name,
                    Surname = "",
                    Username = username,
                    Password = password,
                    Field = field
                };

                entity.vaAccounts.Add(account);
                entity.SaveChanges();

                return account.PK.ToString();
            }
            else
            {
                return "username_exists";
            }

        }

        [HttpPost]
        public string FirmRequests()
        {
            int pk = int.Parse(Request["id"]);

            Entities entity = new Entities();

            vaAccount account = entity.vaAccounts.Where(a => a.PK == pk).FirstOrDefault();

            if (account != null)
            {
                List<Request> listRequests = new List<Request>();

                foreach (vaRequest request in entity.vaRequests.Where(r => !r.Confirmed))
                {
                    if (account.vaOffers.Where(o => o.RequestFK == request.PK && o.AccountFK == pk).FirstOrDefault() != null)
                        continue;

                    listRequests.Add
                    (
                        new Abstract.Request()
                        {
                            pk = request.PK,
                            name = request.vaAccount.Name,
                            surname = request.vaAccount.Surname,
                            date = request.Date,
                            from = request.From,
                            to = request.To,
                            population = request.Population,
                            type = request.Type
                        }
                    );
                }

                return new JavaScriptSerializer().Serialize(listRequests);
            }
            else
            {
                return "account_does_not_exists";
            }
        }

        [HttpPost]
        public string UserOffers()
        {
            int pk = int.Parse(Request["id"]);

            Entities entity = new Entities();

            vaAccount account = entity.vaAccounts.Where(a => a.PK == pk).FirstOrDefault();

            if (account != null)
            {
                List<Offer> listOffers = new List<Offer>();

                foreach (vaRequest request in account.vaRequests.Where(r => !r.Confirmed))
                {
                    foreach (vaOffer offer in request.vaOffers.Where(o => !o.Confirmed).ToList())
                    {
                        listOffers.Add
                        (
                            new Offer()
                            {
                                pk = offer.PK,
                                firm = offer.vaAccount.Name,
                                phone = offer.vaAccount.Field,
                                date = offer.vaRequest.Date,
                                from = offer.vaRequest.From,
                                to = offer.vaRequest.To,
                                population = offer.vaRequest.Population,
                                type = offer.vaRequest.Type,
                                fee = offer.Fee
                            }
                        );
                    }
                }

                return new JavaScriptSerializer().Serialize(listOffers);
            }
            else
            {
                return "account_does_not_exist";
            }
        }

        [HttpPost]
        public string MakeOffer()
        {
            int accountFk = int.Parse(Request["accountId"]);
            int requestFk = int.Parse(Request["requestId"]);
            int fee = int.Parse(Request["fee"]);

            Entities entity = new Entities();

            vaOffer offer = new vaOffer()
            {
                RequestFK = requestFk,
                AccountFK = accountFk,
                Fee = fee,
                Confirmed = false
            };

            entity.vaOffers.Add(offer);
            entity.SaveChanges();

            return "success";
        }

        [HttpPost]
        public string TakeOffer()
        {
            int offerFk = int.Parse(Request["offerId"]);

            Entities entity = new Entities();

            vaOffer offer = entity.vaOffers.Where(o => o.PK == offerFk).FirstOrDefault();

            if (offer != null)
            {
                offer.Confirmed = true;
                offer.vaRequest.Confirmed = true;

                entity.SaveChanges();

                return "success";
            }
            else
            {
                return "error";
            }
        }

        [HttpPost]
        public string Allocations()
        {
            int userId = int.Parse(Request["userId"]);

            Entities entity = new Entities();

            vaAccount user = entity.vaAccounts.Where(a => a.PK == userId).FirstOrDefault();

            List<Offer> allocations = new List<Offer>();
            vaOffer offer;

            foreach (vaRequest request in user.vaRequests.Where(r => r.Confirmed))
            {
                offer = request.vaOffers.Where(o => o.Confirmed).FirstOrDefault();

                allocations.Add
                (
                    new Offer()
                    {
                        pk = offer.PK,
                        firm = offer.vaAccount.Name,
                        phone = offer.vaAccount.Field,
                        date = offer.vaRequest.Date,
                        from = offer.vaRequest.From,
                        to = offer.vaRequest.To,
                        population = offer.vaRequest.Population,
                        type = offer.vaRequest.Type,
                        fee = offer.Fee
                    }
                );
            }

            return new JavaScriptSerializer().Serialize(allocations);
        }

        [HttpPost]
        public string NewRequest()
        {
            int userFk = int.Parse(Request["userId"]);
            string type = Request["type"];
            string date = Request["date"];
            string from = Request["from"];
            string to = Request["to"];
            int population = int.Parse(Request["population"]);

            Entities entity = new Entities();

            vaRequest request = new vaRequest()
            {
                AccountFK = userFk,
                Confirmed = false,
                Date = date,
                From = from,
                Population = population,
                To = to,
                Type = type
            };

            entity.vaRequests.Add(request);
            entity.SaveChanges();

            return "success";
        }
    }
}