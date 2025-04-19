using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Infrastructure.Repositories;
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

        public async Task<IEnumerable<Subject>> GetAllSubjects()
        {
            return await _subjectRepository.GetAllAsNoTracking().ToListAsync();
        }

        public async Task<Subject> GetSubjectById(int id)
        {
            return await _subjectRepository.GetById(id);
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
