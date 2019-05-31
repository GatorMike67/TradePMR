using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TradePMR;

namespace TradePMR.Controllers
{
    public class TradesController : ApiController
    {
        private TradePMREntities db = new TradePMREntities();

        //// GET: api/Trades
        //public IQueryable<Trade> GetTrades()
        //{
        //    return db.Trades;
        //}

        // GET: api/Trades/5
        [ResponseType(typeof(Trade))]
        public IHttpActionResult GetTrade(int id)
        {
            Trade trade = db.Trades.Find(id);
            if (trade == null)
            {
                return NotFound();
            }

            return Ok(trade);
        }


        // GET: api/Trades?params
        [ResponseType(typeof(Trade))]
        [Route("api/TradesQuery/{trade?}/{paging?}")]
        public IHttpActionResult GetTrades([FromUri] Trade trade = null, [FromUri] Paging paging = null)
        {
            try
            {
                Func<Trade, bool> predicate = null;

            
                if (trade == null)
                {
                    return Ok(db.Trades);
                }
                else
                {
                    if (trade.DateCreated != DateTime.MinValue)
                    {
                        predicate = t => t.DateCreated == trade.DateCreated;
                    }

                    else if (trade.Symbol != null)
                    {
                        predicate = t => t.Symbol == trade.Symbol;
                    }

                    else if (trade.Action != null)
                    {
                        predicate = t => t.Action == trade.Action;
                    }

                    else if (trade.AccountId != 0)
                    {
                        predicate = t => t.AccountId == trade.AccountId;
                    }
                }

                if (predicate != null)
                {
                    var trades = db.Trades.Where(predicate).OrderByDescending(a => a.DateCreated);

                    if (trades == null)
                    {
                        return NotFound();
                    }

                    if (paging != null && (paging.Page > 0))
                    {
                        // Get's No of Rows Count   
                        int count = trades.Count();

                        // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
                        int CurrentPage = paging.Page;

                        // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
                        int PageSize = paging.PageSize;

                        // Returns List of Customer after applying Paging   
                        var pagedTrades = trades.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                        return Ok(pagedTrades);
                    }
                    else
                        return Ok(trades);           
                }

                else
                    return BadRequest();
            }

            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // PUT: api/Trades/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTrade(int id, Trade trade)
        {
            if (!ModelState.IsValid || (trade == null))
            {
                return BadRequest(ModelState);
            }

            if (id != trade.ID)
            {
                return BadRequest();
            }

            Trade CurrentTrade = db.Trades.AsNoTracking().Where(t => t.ID == id).FirstOrDefault();
            if (CurrentTrade == null)
            {
                return NotFound();
            }
          
            // Ensure AccountId does not change
            trade.AccountId = CurrentTrade.AccountId;

            // Ensure dates are set
            DateTime dtNow = DateTime.Now;
            trade.DateUpdated = dtNow;

            // This needs to be handled better if the client doesn't send in a full PUT like they're supposed to
            if (trade.DateCreated == DateTime.MinValue)
                trade.DateCreated = dtNow;


            db.Entry(trade).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TradeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Trades
        [ResponseType(typeof(Trade))]
        public IHttpActionResult PostTrade(Trade trade)
        {
            if (!ModelState.IsValid || (trade == null))
            {
                return BadRequest(ModelState);
            }

            try
            {
                DateTime dtNow = DateTime.Now;
                trade.DateCreated = dtNow;
                trade.DateUpdated = dtNow;

                db.Trades.Add(trade);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = trade.ID }, trade);

            }
            catch (Exception ex)
            {
                string Message = ex.Message;

                if (ex.InnerException.InnerException.Message != "")
                    Message += "  " + ex.InnerException.InnerException.Message;

                return Content(HttpStatusCode.BadRequest, Message);
            }
        }

        // DELETE: api/Trades/5
        [ResponseType(typeof(Trade))]
        public IHttpActionResult DeleteTrade(int id)
        {
            Trade trade = db.Trades.Find(id);
            if (trade == null)
            {
                return NotFound();
            }

            db.Trades.Remove(trade);
            db.SaveChanges();

            return Ok(trade);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TradeExists(int id)
        {
            return db.Trades.Count(e => e.ID == id) > 0;
        }
    }
}