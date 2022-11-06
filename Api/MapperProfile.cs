using AutoMapper;
using Common;

namespace Api
{
    public class MapperProfile: Profile
    {
        public MapperProfile() {
            CreateMap<Models.CreateUserModel, DAL.Entities.User>()
                .ForMember(d=>d.Id, m=>m.MapFrom(s=>Guid.NewGuid()))
                .ForMember(d=>d.PasswordHash, m=>m.MapFrom(s=>HashHelper.GetHash(s.Password)))
                .ForMember(d=>d.BirthDay, m=>m.MapFrom(s=>s.BirthDate.UtcDateTime))
                ;

			CreateMap<Models.CreatePostModel, DAL.Entities.Post>()
				.ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
				;

			CreateMap<DAL.Entities.User, Models.UserModel>();

            CreateMap<DAL.Entities.Avatar, Models.AttachModel>();

			CreateMap<DAL.Entities.Post, Models.PostModel>()
                .ForMember(d => d.AuthorId, m => m.MapFrom(m => m.Author));
		}
    }
}
