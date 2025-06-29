using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<List<SubjectDto>> GetAllSubjectDtos()
        {
            var subjects = await _subjectRepository.GetAllAsNoTracking().ToListAsync();
            return _mapper.Map<List<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto> GetSubjectDtoById(int id)
        {
            return _mapper.Map<SubjectDto>(await _subjectRepository.GetById(id));
        }

        public async Task CreateSubject(SaveSubjectDto subjectDto)
        {
            var subject = _mapper.Map<Subject>(subjectDto);
            await _subjectRepository.Add(subject);
        }

        public async Task UpdateSubject(SaveSubjectDto subjectDto)
        {
            var subject = _mapper.Map<Subject>(subjectDto);
            await _subjectRepository.Update(subject);
        }

        public async Task DeleteSubject(int id)
        {
            await _subjectRepository.Delete(id);
        }
    }
}
