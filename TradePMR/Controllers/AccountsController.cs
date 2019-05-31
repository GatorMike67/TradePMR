using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TradePMR;

namespace TradePMR.Controllers
{
    public class AccountsController : ApiController
    {
        private TradePMREntities db = new TradePMREntities();

        // GET: api/Accounts
        public IQueryable<Account> GetAccounts()
        {
            return db.Accounts;
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Account))]
        public IHttpActionResult GetAccount(int id)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // GET: api/Accounts?Name="test"
        [ResponseType(typeof(Account))]
        public IHttpActionResult GetAccount([FromUri] string name, [FromUri] Paging paging = null)
        {
            try
            {
                var accounts = db.Accounts.Where(a => a.Name == name).OrderByDescending(a => a.DateCreated);

                if (accounts == null)
                {
                    return NotFound();
                }

                else
                {
                    if (paging != null && (paging.Page > 0))
                    {
                        // Get's No of Rows Count   
                        int count = accounts.Count();

                        // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                        int CurrentPage = paging.Page;

                        // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                        int PageSize = paging.PageSize;

                        // Returns List of Customer after applying Paging   
                        var pagedAccounts = accounts.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                        return Ok(pagedAccounts);
                    }

                    else
                        return Ok(accounts);
                }
            }

            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // PUT: api/Accounts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAccount(int id, Account account)
        {
            if (!ModelState.IsValid || (account == null))
            {
                return BadRequest(ModelState);
            }

            if (id != account.ID)
            {
                return BadRequest();
            }

            db.Entry(account).State = EntityState.Modified;

            try
            {
                account.DateUpdated = DateTime.Now;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Accounts
        [ResponseType(typeof(Account))]
        public IHttpActionResult PostAccount(Account account)
        {
            if (!ModelState.IsValid || (account == null))
            {
                return BadRequest(ModelState);
            }

            try
            {
                DateTime dtNow = DateTime.Now;
                account.DateCreated = dtNow;
                account.DateUpdated = dtNow;

                db.Accounts.Add(account);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = account.ID }, account);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }   
        }

        // DELETE: api/Accounts/5
        [ResponseType(typeof(Account))]
        public IHttpActionResult DeleteAccount(int id)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(account);
            db.SaveChanges();

            return Ok(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(int id)
        {
            return db.Accounts.Count(e => e.ID == id) > 0;
        }
    }
}