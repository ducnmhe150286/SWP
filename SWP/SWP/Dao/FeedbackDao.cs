using SWP.Dto;
using SWP.Models;

namespace SWP.Dao
{
    public class FeedbackDao
    {
        SWPContext context;

        public FeedbackDao()
        {
           context = new SWPContext();
        }
        public static void AddFeedback(FeedBack feedBack)
        {
            try
            {
                using (var connection = new SWPContext())
                {
                    connection.FeedBacks.Add(feedBack);
                    connection.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<FeedBack> ListFeedback(int productId)
        {
            return context.FeedBacks.Where(x => x.ProductId== productId).ToList();
        }
        public List<FeedbackViewModel> ListFeedbackViewModel(int productId)
        {
            var model = (from a in context.FeedBacks
                         join b in context.Users
                         on a.UserId equals b.UserId
                         where a.ProductId == productId

                         select new
                         {
                             FeedBackId = a.FeedBackId,
                             Comment = a.Comment,
                             UserId = b.UserId,
                             Email = b.Email,
                             CreatedDate = a.CreatedDate,
                             ProductId = a.ProductId,
                             ProductName = a.Product.ProductName,
                             Rate = a.Rating,
                             Status = a.Status

                         }).AsEnumerable().Select(x => new FeedbackViewModel()
                         {
                             FeedBackId = x.FeedBackId,
                             Comment = x.Comment,
                             Email = x.Email,
                             CreatedDate = x.CreatedDate,
                             ProductName = x.ProductName,
                             Rating = x.Rate,
                             Status = x.Status
                         }) ;
            return model.OrderByDescending(y => y.FeedBackId).ToList();
        }
    }
}
