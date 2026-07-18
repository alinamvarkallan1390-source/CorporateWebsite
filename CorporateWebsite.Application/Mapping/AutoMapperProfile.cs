using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using AutoMapper;

namespace CorporateWebsite.Application.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Language
        CreateMap<Language, LanguageDto>().ReverseMap();
        CreateMap<CreateLanguageDto, Language>();
        CreateMap<UpdateLanguageDto, Language>();

        // Page
        CreateMap<Page, PageDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<PageTranslation, PageTranslationDto>().ReverseMap();
        CreateMap<CreatePageDto, Page>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreatePageTranslationDto, PageTranslation>();
        CreateMap<UpdatePageDto, Page>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdatePageTranslationDto, PageTranslation>();

        // Service
        CreateMap<Service, ServiceDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Features, opt => opt.MapFrom(s => s.Features))
            .ForMember(d => d.Faqs, opt => opt.MapFrom(s => s.Faqs))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images));
        CreateMap<ServiceTranslation, ServiceTranslationDto>().ReverseMap();
        CreateMap<ServiceFeature, ServiceFeatureDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<ServiceFeatureTranslation, ServiceFeatureTranslationDto>().ReverseMap();
        CreateMap<ServiceFaq, ServiceFaqDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<ServiceFaqTranslation, ServiceFaqTranslationDto>().ReverseMap();
        CreateMap<ServiceImage, ServiceImageDto>().ReverseMap();
        CreateMap<CreateServiceDto, Service>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Features, opt => opt.MapFrom(s => s.Features))
            .ForMember(d => d.Faqs, opt => opt.MapFrom(s => s.Faqs))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images));
        CreateMap<CreateServiceTranslationDto, ServiceTranslation>();
        CreateMap<CreateServiceFeatureDto, ServiceFeature>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateServiceFeatureTranslationDto, ServiceFeatureTranslation>();
        CreateMap<CreateServiceFaqDto, ServiceFaq>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateServiceFaqTranslationDto, ServiceFaqTranslation>();
        CreateMap<CreateServiceImageDto, ServiceImage>();
        CreateMap<UpdateServiceDto, Service>()
            .ForMember(d => d.Translations, opt => opt.Ignore())
            .ForMember(d => d.Features, opt => opt.Ignore())
            .ForMember(d => d.Faqs, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore());
        CreateMap<UpdateServiceTranslationDto, ServiceTranslation>();
        CreateMap<UpdateServiceFeatureDto, ServiceFeature>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateServiceFeatureTranslationDto, ServiceFeatureTranslation>();
        CreateMap<UpdateServiceFaqDto, ServiceFaq>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateServiceFaqTranslationDto, ServiceFaqTranslation>();
        CreateMap<UpdateServiceImageDto, ServiceImage>();

        // Service Category
        CreateMap<ServiceCategory, ServiceCategoryDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children));
        CreateMap<ServiceCategoryTranslation, ServiceCategoryTranslationDto>().ReverseMap();
        CreateMap<CreateServiceCategoryDto, ServiceCategory>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateServiceCategoryTranslationDto, ServiceCategoryTranslation>();
        CreateMap<UpdateServiceCategoryDto, ServiceCategory>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateServiceCategoryTranslationDto, ServiceCategoryTranslation>();

        // Project
        CreateMap<Project, ProjectDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos))
            .ForMember(d => d.Files, opt => opt.MapFrom(s => s.Files))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.ProjectTags.Select(pt => pt.Tag)))
            .ForMember(d => d.Features, opt => opt.MapFrom(s => s.Features))
            .ForMember(d => d.TeamMembers, opt => opt.MapFrom(s => s.TeamMembers));
        CreateMap<ProjectTranslation, ProjectTranslationDto>().ReverseMap();
        CreateMap<ProjectImage, ProjectImageDto>().ReverseMap();
        CreateMap<ProjectVideo, ProjectVideoDto>().ReverseMap();
        CreateMap<ProjectFile, ProjectFileDto>().ReverseMap();
        CreateMap<ProjectFeature, ProjectFeatureDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<ProjectFeatureTranslation, ProjectFeatureTranslationDto>().ReverseMap();
        CreateMap<ProjectTeamMember, ProjectTeamMemberDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<ProjectTeamMemberTranslation, ProjectTeamMemberTranslationDto>().ReverseMap();
        CreateMap<CreateProjectDto, Project>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos))
            .ForMember(d => d.Files, opt => opt.MapFrom(s => s.Files))
            .ForMember(d => d.ProjectTags, opt => opt.MapFrom(s => s.TagIds.Select(id => new ProjectTag { TagId = id })))
            .ForMember(d => d.Features, opt => opt.MapFrom(s => s.Features))
            .ForMember(d => d.TeamMembers, opt => opt.MapFrom(s => s.TeamMembers));
        CreateMap<CreateProjectTranslationDto, ProjectTranslation>();
        CreateMap<CreateProjectImageDto, ProjectImage>();
        CreateMap<CreateProjectVideoDto, ProjectVideo>();
        CreateMap<CreateProjectFileDto, ProjectFile>();
        CreateMap<CreateProjectFeatureDto, ProjectFeature>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateProjectFeatureTranslationDto, ProjectFeatureTranslation>();
        CreateMap<CreateProjectTeamMemberDto, ProjectTeamMember>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateProjectTeamMemberTranslationDto, ProjectTeamMemberTranslation>();
        CreateMap<UpdateProjectDto, Project>()
            .ForMember(d => d.Translations, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore())
            .ForMember(d => d.Videos, opt => opt.Ignore())
            .ForMember(d => d.Files, opt => opt.Ignore())
            .ForMember(d => d.ProjectTags, opt => opt.Ignore())
            .ForMember(d => d.Features, opt => opt.Ignore())
            .ForMember(d => d.TeamMembers, opt => opt.Ignore());
        CreateMap<UpdateProjectTranslationDto, ProjectTranslation>();
        CreateMap<UpdateProjectImageDto, ProjectImage>();
        CreateMap<UpdateProjectVideoDto, ProjectVideo>();
        CreateMap<UpdateProjectFileDto, ProjectFile>();
        CreateMap<UpdateProjectFeatureDto, ProjectFeature>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateProjectFeatureTranslationDto, ProjectFeatureTranslation>();
        CreateMap<UpdateProjectTeamMemberDto, ProjectTeamMember>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateProjectTeamMemberTranslationDto, ProjectTeamMemberTranslation>();

        // Project Category
        CreateMap<ProjectCategory, ProjectCategoryDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children));
        CreateMap<ProjectCategoryTranslation, ProjectCategoryTranslationDto>().ReverseMap();
        CreateMap<CreateProjectCategoryDto, ProjectCategory>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateProjectCategoryTranslationDto, ProjectCategoryTranslation>();
        CreateMap<UpdateProjectCategoryDto, ProjectCategory>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateProjectCategoryTranslationDto, ProjectCategoryTranslation>();

        // News
        CreateMap<News, NewsDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.NewsTags.Select(nt => nt.Tag)))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos))
            .ForMember(d => d.Files, opt => opt.MapFrom(s => s.Files));
        CreateMap<NewsTranslation, NewsTranslationDto>().ReverseMap();
        CreateMap<NewsImage, NewsImageDto>().ReverseMap();
        CreateMap<NewsVideo, NewsVideoDto>().ReverseMap();
        CreateMap<NewsFile, NewsFileDto>().ReverseMap();
        CreateMap<CreateNewsDto, News>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.NewsTags, opt => opt.MapFrom(s => s.TagIds.Select(id => new NewsTag { TagId = id })))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images))
            .ForMember(d => d.Videos, opt => opt.MapFrom(s => s.Videos))
            .ForMember(d => d.Files, opt => opt.MapFrom(s => s.Files))
            .ForMember(d => d.RelatedNews, opt => opt.MapFrom(s => s.RelatedNewsIds.Select(id => new RelatedNews { RelatedNewsId = id })));
        CreateMap<CreateNewsTranslationDto, NewsTranslation>();
        CreateMap<CreateNewsImageDto, NewsImage>();
        CreateMap<CreateNewsVideoDto, NewsVideo>();
        CreateMap<CreateNewsFileDto, NewsFile>();
        CreateMap<UpdateNewsDto, News>()
            .ForMember(d => d.Translations, opt => opt.Ignore())
            .ForMember(d => d.NewsTags, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore())
            .ForMember(d => d.Videos, opt => opt.Ignore())
            .ForMember(d => d.Files, opt => opt.Ignore())
            .ForMember(d => d.RelatedNews, opt => opt.Ignore());
        CreateMap<UpdateNewsTranslationDto, NewsTranslation>();
        CreateMap<UpdateNewsImageDto, NewsImage>();
        CreateMap<UpdateNewsVideoDto, NewsVideo>();
        CreateMap<UpdateNewsFileDto, NewsFile>();

        // News Category
        CreateMap<NewsCategory, NewsCategoryDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children));
        CreateMap<NewsCategoryTranslation, NewsCategoryTranslationDto>().ReverseMap();
        CreateMap<CreateNewsCategoryDto, NewsCategory>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateNewsCategoryTranslationDto, NewsCategoryTranslation>();
        CreateMap<UpdateNewsCategoryDto, NewsCategory>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateNewsCategoryTranslationDto, NewsCategoryTranslation>();

        // Tag
        CreateMap<Tag, TagDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<TagTranslation, TagTranslationDto>().ReverseMap();
        CreateMap<CreateTagDto, Tag>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateTagTranslationDto, TagTranslation>();
        CreateMap<UpdateTagDto, Tag>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateTagTranslationDto, TagTranslation>();

        // Menu
        CreateMap<Menu, MenuDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));
        CreateMap<MenuTranslation, MenuTranslationDto>().ReverseMap();
        CreateMap<MenuItem, MenuItemDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children))
            .ForMember(d => d.Page, opt => opt.MapFrom(s => s.Page))
            .ForMember(d => d.Service, opt => opt.MapFrom(s => s.Service))
            .ForMember(d => d.Project, opt => opt.MapFrom(s => s.Project))
            .ForMember(d => d.NewsCategory, opt => opt.MapFrom(s => s.NewsCategory));
        CreateMap<MenuItemTranslation, MenuItemTranslationDto>().ReverseMap();
        CreateMap<CreateMenuDto, Menu>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateMenuTranslationDto, MenuTranslation>();
        CreateMap<UpdateMenuDto, Menu>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateMenuTranslationDto, MenuTranslation>();
        CreateMap<CreateMenuItemDto, MenuItem>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateMenuItemTranslationDto, MenuItemTranslation>();
        CreateMap<UpdateMenuItemDto, MenuItem>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateMenuItemTranslationDto, MenuItemTranslation>();

        // Setting
        CreateMap<Setting, SettingDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<SettingTranslation, SettingTranslationDto>().ReverseMap();
        CreateMap<UpdateSettingDto, Setting>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateSettingTranslationDto, SettingTranslation>();

        // Form
        CreateMap<Form, FormDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Fields, opt => opt.MapFrom(s => s.Fields));
        CreateMap<FormTranslation, FormTranslationDto>().ReverseMap();
        CreateMap<FormField, FormFieldDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<FormFieldTranslation, FormFieldTranslationDto>().ReverseMap();
        CreateMap<FormSubmission, FormSubmissionDto>()
            .ForMember(d => d.FieldValues, opt => opt.MapFrom(s => s.FieldValues))
            .ForMember(d => d.Files, opt => opt.MapFrom(s => s.Files));
        CreateMap<FormFieldValue, FormFieldValueDto>().ReverseMap();
        CreateMap<FormSubmissionFile, FormSubmissionFileDto>().ReverseMap();
        CreateMap<CreateFormDto, Form>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Fields, opt => opt.MapFrom(s => s.Fields));
        CreateMap<CreateFormTranslationDto, FormTranslation>();
        CreateMap<CreateFormFieldDto, FormField>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateFormFieldTranslationDto, FormFieldTranslation>();
        CreateMap<UpdateFormDto, Form>()
            .ForMember(d => d.Translations, opt => opt.Ignore())
            .ForMember(d => d.Fields, opt => opt.Ignore());
        CreateMap<UpdateFormTranslationDto, FormTranslation>();
        CreateMap<UpdateFormFieldDto, FormField>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateFormFieldTranslationDto, FormFieldTranslation>();

        // Slider
        CreateMap<Slider, SliderDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));
        CreateMap<SliderTranslation, SliderTranslationDto>().ReverseMap();
        CreateMap<SliderItem, SliderItemDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<SliderItemTranslation, SliderItemTranslationDto>().ReverseMap();
        CreateMap<CreateSliderDto, Slider>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));
        CreateMap<CreateSliderTranslationDto, SliderTranslation>();
        CreateMap<CreateSliderItemDto, SliderItem>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateSliderItemTranslationDto, SliderItemTranslation>();
        CreateMap<UpdateSliderDto, Slider>()
            .ForMember(d => d.Translations, opt => opt.Ignore())
            .ForMember(d => d.Items, opt => opt.Ignore());
        CreateMap<UpdateSliderTranslationDto, SliderTranslation>();
        CreateMap<UpdateSliderItemDto, SliderItem>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateSliderItemTranslationDto, SliderItemTranslation>();

        // Banner
        CreateMap<Banner, BannerDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<BannerTranslation, BannerTranslationDto>().ReverseMap();
        CreateMap<CreateBannerDto, Banner>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateBannerTranslationDto, BannerTranslation>();
        CreateMap<UpdateBannerDto, Banner>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateBannerTranslationDto, BannerTranslation>();

        // Media
        CreateMap<MediaFile, MediaFileDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<MediaFileTranslation, MediaFileTranslationDto>().ReverseMap();
        CreateMap<MediaFolder, MediaFolderDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations))
            .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children));
        CreateMap<MediaFolderTranslation, MediaFolderTranslationDto>().ReverseMap();
        CreateMap<CreateMediaFolderDto, MediaFolder>()
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<CreateMediaFolderTranslationDto, MediaFolderTranslation>();
        CreateMap<UpdateMediaFolderDto, MediaFolder>()
            .ForMember(d => d.Translations, opt => opt.Ignore());
        CreateMap<UpdateMediaFolderTranslationDto, MediaFolderTranslation>();

        // Redirect
        CreateMap<Redirect, RedirectDto>().ReverseMap();
        CreateMap<CreateRedirectDto, Redirect>();
        CreateMap<UpdateRedirectDto, Redirect>();
        CreateMap<BrokenLink, BrokenLinkDto>().ReverseMap();

        // Activity Log
        CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();

        // Backup
        CreateMap<Backup, BackupDto>().ReverseMap();

        // Scheduled Task
        CreateMap<ScheduledTask, ScheduledTaskDto>().ReverseMap();
        CreateMap<CreateScheduledTaskDto, ScheduledTask>();
        CreateMap<UpdateScheduledTaskDto, ScheduledTask>();
        CreateMap<ScheduledTaskLog, ScheduledTaskLogDto>().ReverseMap();

        // Email Template
        CreateMap<EmailTemplate, EmailTemplateDto>()
            .ForMember(d => d.Translation, opt => opt.MapFrom(s => s.Translations.FirstOrDefault()))
            .ForMember(d => d.Translations, opt => opt.MapFrom(s => s.Translations));
        CreateMap<EmailTemplateTranslation, EmailTemplateTranslationDto>().ReverseMap();

        // Email Queue
        CreateMap<EmailQueue, EmailQueueDto>().ReverseMap();

        // User/Role
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
        CreateMap<ApplicationRole, RoleDto>().ReverseMap();
        CreateMap<RolePermission, RolePermissionDto>().ReverseMap();

        // Reorder
        CreateMap<ReorderItemDto, ReorderItemDto>();
    }
}

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int? LanguageId { get; set; }
    public string? TimeZone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public ICollection<string> Roles { get; set; } = new List<string>();
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<string> Permissions { get; set; } = new List<string>();
}

public class RolePermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}