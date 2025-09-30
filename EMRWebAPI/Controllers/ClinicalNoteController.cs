using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClinicalNoteController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ClinicalNoteController> _logger;

        public ClinicalNoteController(EMRDBContext context, ILogger<ClinicalNoteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient clinical notes
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<ClinicalNote>>> GetPatientNotes(int patientId)
        {
            try
            {
                var notes = await _context.ClinicalNotes
                    .Include(n => n.Provider)
                    .Include(n => n.Encounter)
                    .Where(n => n.PatientId == patientId)
                    .OrderByDescending(n => n.NoteDate)
                    .ToListAsync();

                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving notes for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving clinical notes" });
            }
        }

        /// <summary>
        /// Get clinical note by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<ClinicalNote>> GetClinicalNote(int id)
        {
            try
            {
                var note = await _context.ClinicalNotes
                    .Include(n => n.Patient)
                    .Include(n => n.Provider)
                    .Include(n => n.Encounter)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (note == null)
                {
                    return NotFound(new { message = "Clinical note not found" });
                }

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving clinical note {id}");
                return StatusCode(500, new { message = "Error retrieving clinical note" });
            }
        }

        /// <summary>
        /// Create SOAP note
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<ClinicalNote>> CreateClinicalNote([FromBody] ClinicalNote note)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                note.NoteDate = DateTime.UtcNow;
                note.Status = "Draft";
                note.CreatedDate = DateTime.UtcNow;
                note.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.ClinicalNotes.Add(note);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Clinical note created: {note.Id}");

                return CreatedAtAction(nameof(GetClinicalNote), new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinical note");
                return StatusCode(500, new { message = "Error creating clinical note" });
            }
        }

        /// <summary>
        /// Update clinical note
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateClinicalNote(int id, [FromBody] ClinicalNote note)
        {
            try
            {
                if (id != note.Id)
                {
                    return BadRequest(new { message = "Note ID mismatch" });
                }

                var existingNote = await _context.ClinicalNotes.FindAsync(id);
                if (existingNote == null)
                {
                    return NotFound(new { message = "Clinical note not found" });
                }

                // Only allow updates if not signed
                if (existingNote.IsSigned)
                {
                    return BadRequest(new { message = "Cannot modify signed note. Create addendum instead." });
                }

                existingNote.Subjective = note.Subjective;
                existingNote.Objective = note.Objective;
                existingNote.Assessment = note.Assessment;
                existingNote.Plan = note.Plan;
                existingNote.ChiefComplaint = note.ChiefComplaint;
                existingNote.HistoryOfPresentIllness = note.HistoryOfPresentIllness;
                existingNote.PhysicalExam = note.PhysicalExam;
                existingNote.ModifiedDate = DateTime.UtcNow;
                existingNote.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating clinical note {id}");
                return StatusCode(500, new { message = "Error updating clinical note" });
            }
        }

        /// <summary>
        /// Sign clinical note
        /// </summary>
        [HttpPost("{id}/sign")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> SignClinicalNote(int id)
        {
            try
            {
                var note = await _context.ClinicalNotes.FindAsync(id);
                if (note == null)
                {
                    return NotFound(new { message = "Clinical note not found" });
                }

                if (note.IsSigned)
                {
                    return BadRequest(new { message = "Note already signed" });
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                note.IsSigned = true;
                note.SignedDate = DateTime.UtcNow;
                note.SignedBy = userId;
                note.Status = "Signed";

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Clinical note signed: {id} by {userId}");

                return Ok(new { message = "Note signed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error signing clinical note {id}");
                return StatusCode(500, new { message = "Error signing clinical note" });
            }
        }

        /// <summary>
        /// Create addendum to signed note
        /// </summary>
        [HttpPost("{id}/addendum")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<ClinicalNote>> CreateAddendum(
            int id,
            [FromBody] string addendumText)
        {
            try
            {
                var originalNote = await _context.ClinicalNotes.FindAsync(id);
                if (originalNote == null)
                {
                    return NotFound(new { message = "Original note not found" });
                }

                var addendum = new ClinicalNote
                {
                    PatientId = originalNote.PatientId,
                    ProviderId = originalNote.ProviderId,
                    EncounterId = originalNote.EncounterId,
                    NoteType = originalNote.NoteType,
                    NoteDate = DateTime.UtcNow,
                    Assessment = $"ADDENDUM to note {id}: {addendumText}",
                    IsAddendum = true,
                    ParentNoteId = id,
                    Status = "Draft",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                };

                _context.ClinicalNotes.Add(addendum);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Addendum created for note {id}");

                return Ok(addendum);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating addendum for note {id}");
                return StatusCode(500, new { message = "Error creating addendum" });
            }
        }
    }
}